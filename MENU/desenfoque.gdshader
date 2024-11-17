shader_type canvas_item;

uniform float radius : hint_range(0.0, 10.0) = 5.0;
uniform vec2 direction = vec2(1.0, 0.0);

void fragment() {
    vec4 color = vec4(0.0);
    vec2 texel_size = TEXTURE_PIXEL_SIZE * radius;
    int iterations = 6;

    for (int i = -iterations; i <= iterations; i++) {
        color += texture(TEXTURE, UV + direction * texel_size * float(i)) / float(iterations * 2 + 1);
    }

    COLOR = color;
}
