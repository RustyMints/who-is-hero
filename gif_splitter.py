from PIL import Image
import os

def split_gif(gif_path, output_dir=None, output_format="png"):
    if not os.path.exists(gif_path):
        print(f"错误：文件不存在 - {gif_path}")
        return

    if output_dir is None:
        output_dir = os.path.dirname(gif_path)
        if not output_dir:
            output_dir = "."

    os.makedirs(output_dir, exist_ok=True)

    img = Image.open(gif_path)
    gif_name = os.path.splitext(os.path.basename(gif_path))[0]

    frame_count = 0
    try:
        while True:
            frame_path = os.path.join(output_dir, f"{gif_name}_frame_{frame_count:04d}.{output_format}")
            img.save(frame_path, output_format.upper())
            frame_count += 1
            img.seek(img.tell() + 1)
    except EOFError:
        pass

    print(f"切割完成！共 {frame_count} 帧，保存到：{output_dir}")

if __name__ == "__main__":
    import sys
    if len(sys.argv) < 2:
        print("用法：python gif_splitter.py <gif文件路径> [输出目录] [输出格式]")
        print("示例：python gif_splitter.py animation.gif frames png")
        sys.exit(1)
    
    gif_path = sys.argv[1]
    output_dir = sys.argv[2] if len(sys.argv) > 2 else None
    output_format = sys.argv[3] if len(sys.argv) > 3 else "png"
    
    split_gif(gif_path, output_dir, output_format)
