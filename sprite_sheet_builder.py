from PIL import Image
import os
import json
import math

def build_sprite_sheet(input_dir, output_path, max_columns=8, padding=2, background_color=(0, 0, 0, 0)):
    image_files = sorted([f for f in os.listdir(input_dir) if f.lower().endswith(('.png', '.jpg', '.jpeg'))])
    
    if not image_files:
        print(f"错误：{input_dir} 中没有图片文件")
        return None
    
    first_image = Image.open(os.path.join(input_dir, image_files[0]))
    frame_width, frame_height = first_image.size
    
    num_frames = len(image_files)
    num_columns = min(max_columns, num_frames)
    num_rows = math.ceil(num_frames / num_columns)
    
    sheet_width = num_columns * frame_width + (num_columns + 1) * padding
    sheet_height = num_rows * frame_height + (num_rows + 1) * padding
    
    sprite_sheet = Image.new('RGBA', (sheet_width, sheet_height), background_color)
    
    frames_info = []
    
    for i, filename in enumerate(image_files):
        img_path = os.path.join(input_dir, filename)
        img = Image.open(img_path).convert('RGBA')
        
        col = i % num_columns
        row = i // num_columns
        
        x = padding + col * (frame_width + padding)
        y = padding + row * (frame_height + padding)
        
        sprite_sheet.paste(img, (x, y))
        
        frames_info.append({
            'filename': filename,
            'frame': {
                'x': x,
                'y': y,
                'width': frame_width,
                'height': frame_height
            },
            'rotated': False,
            'trimmed': False,
            'spriteSourceSize': {
                'x': 0,
                'y': 0,
                'width': frame_width,
                'height': frame_height
            },
            'sourceSize': {
                'width': frame_width,
                'height': frame_height
            }
        })
    
    sprite_sheet.save(output_path, 'PNG')
    
    output_dir = os.path.dirname(output_path)
    output_name = os.path.splitext(os.path.basename(output_path))[0]
    json_path = os.path.join(output_dir, f"{output_name}.json")
    
    with open(json_path, 'w', encoding='utf-8') as f:
        json.dump({
            'frames': {f['filename']: f for f in frames_info},
            'meta': {
                'image': os.path.basename(output_path),
                'size': {'w': sheet_width, 'h': sheet_height},
                'scale': 1,
                'format': 'RGBA8888'
            }
        }, f, indent=2, ensure_ascii=False)
    
    print(f"精灵表生成完成！")
    print(f"  图片: {output_path}")
    print(f"  配置: {json_path}")
    print(f"  帧数: {num_frames}")
    print(f"  尺寸: {sheet_width} x {sheet_height}")
    print(f"  排列: {num_columns}列 x {num_rows}行")
    
    return output_path

def batch_build_sprite_sheets(input_dirs, output_root, max_columns=8, padding=2):
    for input_dir in input_dirs:
        if not os.path.exists(input_dir):
            print(f"跳过：{input_dir} 不存在")
            continue
        
        dir_name = os.path.basename(input_dir).replace('_frames', '')
        output_path = os.path.join(output_root, f"{dir_name}_sprite_sheet.png")
        
        build_sprite_sheet(input_dir, output_path, max_columns, padding)

if __name__ == "__main__":
    import sys
    
    if len(sys.argv) < 3:
        print("用法1: python sprite_sheet_builder.py <输入文件夹> <输出精灵表路径>")
        print("用法2: python sprite_sheet_builder.py batch <输入文件夹1> <输入文件夹2> ... <输出根目录>")
        print("示例: python sprite_sheet_builder.py frames/ animation_spritesheet.png")
        print("示例: python sprite_sheet_builder.py batch frames1/ frames2/ output/")
        sys.exit(1)
    
    if sys.argv[1] == 'batch':
        input_dirs = sys.argv[2:-1]
        output_root = sys.argv[-1]
        os.makedirs(output_root, exist_ok=True)
        batch_build_sprite_sheets(input_dirs, output_root)
    else:
        input_dir = sys.argv[1]
        output_path = sys.argv[2]
        build_sprite_sheet(input_dir, output_path)
