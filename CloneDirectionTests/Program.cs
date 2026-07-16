using System;
using System.Collections.Generic;

// ============ TDD GREEN 测试桩：复刻修复后生产代码的 SetupClone + FaceClosestTarget ============

public class TransformStub
{
    public Vector3Stub position;
    public Vector3Stub eulerAngles;
    public TransformStub(Vector3Stub pos) { position = pos; eulerAngles = new Vector3Stub(0,0,0); }
}
public struct Vector3Stub
{
    public float x, y, z;
    public Vector3Stub(float x, float y, float z) { this.x=x; this.y=y; this.z=z; }
    public static Vector3Stub operator +(Vector3Stub a, Vector3Stub b) => new Vector3Stub(a.x+b.x, a.y+b.y, a.z+b.z);
}

public class SkillMgrStub { public PlayerStub player; }

public class PlayerStub
{
    public int facingDir;
}

public class CloneControllerStub
{
    public TransformStub transform = new TransformStub(new Vector3Stub(0,0,0));
    public int facingDir = 1;

    private TransformStub closesEnemy;
    private PlayerStub player;

    public void SetupClone(TransformStub _newTransform, Vector3Stub _offset,
                           TransformStub _closesEnemy, PlayerStub _player)
    {
        player = _player;
        transform.position = _newTransform.position + _offset;
        closesEnemy = _closesEnemy;
        FaceClosestTarget();
    }

    // GREEN：完全对应修复后生产代码的 FaceClosestTarget（先清 rotation + 有目标用目标 无目标fallback玩家）
    private void FaceClosestTarget()
    {
        // 修复C：翻转前先清 rotation.y=0，避免之前 Rotate 累加
        transform.eulerAngles.y = 0;

        if (closesEnemy != null)
        {
            if (transform.position.x > closesEnemy.position.x)
            {
                facingDir = -1;
                transform.eulerAngles.y = 180;   // 直接赋值 180，而不是 += 累加
            }
            else
            {
                // 右分支：已在函数开头清 y=0，直接 facingDir=1 朝右
                facingDir = 1;
            }
        }
        else
        {
            // closesEnemy 真的 null（25 半径无敌人）→ fallback 玩家朝向（这是合理行为）
            facingDir = SkillMgrStub_instance.player.facingDir;
            if (facingDir == -1)
            {
                transform.eulerAngles.y = 180;
            }
        }
    }

    public static PlayerStub SkillMgrStub_instance_player { set { SkillMgrStub_instance = new SkillMgrStub { player = value }; } }
    public static SkillMgrStub SkillMgrStub_instance;
}

// ============ 测试入口 ============
class Program
{
    static int failed, total;
    static void Assert(string caseName, int expected, int actual)
    {
        total++;
        bool pass = expected == actual;
        if (pass) Console.WriteLine($"  [PASS] {caseName}");
        else { failed++; Console.WriteLine($"  [FAIL] {caseName}: expected facingDir={expected}, actual={actual}"); }
    }
    static void Assert(string caseName, string expected, string actual)
    {
        total++;
        bool pass = expected == actual;
        if (pass) Console.WriteLine($"  [PASS] {caseName}");
        else { failed++; Console.WriteLine($"  [FAIL] {caseName}: expected=\"{expected}\", actual=\"{actual}\""); }
    }
    static int Main()
    {
        var player = new PlayerStub { facingDir = 1 };
        CloneControllerStub.SkillMgrStub_instance_player = player;

        // =============================================
        // 场景 1（修复后镜像反击核心路径）：closesEnemy=反击命中的特定敌人，镜像在敌人右侧(x=12)身后，面朝敌人左 -1
        // =============================================
        Console.WriteLine("--- 场景1：镜像反击路径（closesEnemy=命中敌人） 镜像在敌人右侧身后 → 面朝敌人 -1 ---");
        {
            var clone = new CloneControllerStub();
            var enemy = new TransformStub(new Vector3Stub(10,0,0));
            var offset = new Vector3Stub(2, 0, 0);
            clone.SetupClone(enemy, offset, enemy, player);
            Assert("镜像在敌人右侧 + closesEnemy=敌人 → 面朝敌人 -1", expected: -1, actual: clone.facingDir);
            Assert("同一场景：rotation.y = 180°", expected: "180", actual: clone.transform.eulerAngles.y.ToString());
        }

        // =============================================
        // 场景 2：closesEnemy 正确，镜像在敌人左侧 → 面朝敌人右 +1
        // =============================================
        Console.WriteLine();
        Console.WriteLine("--- 场景2：closesEnemy=敌人 镜像在敌人左侧 → +1 ---");
        {
            var clone = new CloneControllerStub();
            var enemy = new TransformStub(new Vector3Stub(10,0,0));
            var offset = new Vector3Stub(-2, 0, 0);
            clone.SetupClone(enemy, offset, enemy, player);
            Assert("镜像在敌人左侧 → 面朝敌人 +1", expected: 1, actual: clone.facingDir);
            Assert("同一场景 rotation.y = 0°", expected: "0", actual: clone.transform.eulerAngles.y.ToString());
        }

        // =============================================
        // 场景 3（验证玩家朝左）：closesEnemy=null 玩家朝左 → fallback 朝左 -1
        // =============================================
        Console.WriteLine();
        Console.WriteLine("--- 场景3：closesEnemy=null（真没敌人） 玩家朝左 → fallback朝左 -1 ---");
        {
            var clone = new CloneControllerStub();
            var playerLeft = new PlayerStub { facingDir = -1 };
            CloneControllerStub.SkillMgrStub_instance_player = playerLeft;
            var enemy = new TransformStub(new Vector3Stub(10,0,0));
            clone.SetupClone(enemy, new Vector3Stub(2,0,0), null, playerLeft);
            Assert("closesEnemy=null + 玩家朝左 → fallback 朝左 -1", expected: -1, actual: clone.facingDir);
            Assert("同一场景 rotation.y = 180°", expected: "180", actual: clone.transform.eulerAngles.y.ToString());
            CloneControllerStub.SkillMgrStub_instance_player = player; // 恢复
        }

        // =============================================
        // 场景 4（rotation 累加防御）：同1个clone先朝左（180°）再朝右（0°） → 第二次必须 rotation.y=0（无残留）
        // 修复前会是 180+0=180 残留；修复后先清零再赋值 → 0°
        // =============================================
        Console.WriteLine();
        Console.WriteLine("--- 场景4：rotation累加防御（同对象先左后右） ---");
        {
            var clone = new CloneControllerStub();
            var enemy = new TransformStub(new Vector3Stub(10,0,0));
            clone.SetupClone(enemy, new Vector3Stub(2,0,0), enemy, player);  // 第1次：右侧 朝左 → 180
            clone.SetupClone(enemy, new Vector3Stub(-2,0,0), enemy, player); // 第2次：左侧 朝右 → 应清到 0
            Assert("第2次朝右后 facingDir=1", expected: 1, actual: clone.facingDir);
            Assert("第2次朝右后 rotation.y=0（无180°残留）", expected: "0", actual: clone.transform.eulerAngles.y.ToString());
        }

        // =============================================
        // 场景 5（玩家朝左，镜像在敌人身后，closesEnemy=敌人）：玩家朝左时镜像在敌人左侧2格（=身后） → 面朝右 +1 朝敌人
        // =============================================
        Console.WriteLine();
        Console.WriteLine("--- 场景5：玩家朝左，镜像在敌人左侧身后 closesEnemy=敌人 → 面朝敌人右 +1 ---");
        {
            var clone = new CloneControllerStub();
            var playerLeft = new PlayerStub { facingDir = -1 };
            CloneControllerStub.SkillMgrStub_instance_player = playerLeft;
            var enemy = new TransformStub(new Vector3Stub(10,0,0));
            // 玩家朝左（facingDir=-1），offset = 2*(-1) = -2 → 镜像生成在 x=8，敌人左侧（身后）
            var offset = new Vector3Stub(-2, 0, 0);
            clone.SetupClone(enemy, offset, enemy, playerLeft);
            Assert("镜像在敌人左侧身后 + closesEnemy=敌人 → 面朝敌人 +1", expected: 1, actual: clone.facingDir);
            Assert("同一场景 rotation.y=0°", expected: "0", actual: clone.transform.eulerAngles.y.ToString());
            CloneControllerStub.SkillMgrStub_instance_player = player; // 恢复
        }

        Console.WriteLine();
        Console.WriteLine($"总计 {total} 项，失败 {failed} 项。");
        if (failed == 0) Console.WriteLine("!!! GREEN：所有场景全 PASS ✅ !!!");
        return failed == 0 ? 0 : 1;
    }
}
