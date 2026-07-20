using UnityEngine;

public class CheckPoint : MonoBehaviour
{
    private Animator anim;
    public string id;
    public bool activationStatus;

    private void Awake()
    {
        anim = GetComponent<Animator>();

        if (string.IsNullOrEmpty(id))
        {
            id = GenerateStableId();
        }
    }

    private string GenerateStableId()
    {
        string path = gameObject.scene.name + "/" + GetTransformPath(transform);
        return Mathf.Abs(path.GetHashCode()).ToString();
    }

    private string GetTransformPath(Transform t)
    {
        string path = t.name;
        while (t.parent != null)
        {
            t = t.parent;
            path = t.name + "/" + path;
        }
        return path;
    }

    [ContextMenu("生成唯一ID")]
    private void GenerateId()
    {
        id = System.Guid.NewGuid().ToString();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.GetComponent<Player>() != null)
        {
            ActivateCheckPoint();
        }
    }

    public void ActivateCheckPoint()
    {
        if(activationStatus == false)
            AudioManager.instance.PlaySFX(13,transform);

        activationStatus = true;
        if (anim != null)
            anim.SetBool("active", true);
    }
}
