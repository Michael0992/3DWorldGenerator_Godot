shader_type spatial;

uniform sampler2D grass_texture : source_color;
uniform sampler2D forest_texture : source_color;
uniform sampler2D rock_texture : source_color;
uniform float rock_height = 5.0;
uniform float blend_range = 2.0;
uniform float texture_scale = 0.1;

varying vec3 world_pos;
varying vec3 world_normal;

// Zufälliger Wert pro Gitterzelle
float hash(vec2 p) {
    return fract(sin(dot(p, vec2(127.1, 311.7))) * 43758.5453);
}

// Weiches Rauschen ohne Streifen
float smoothNoise(vec2 p) {
    vec2 i = floor(p);
    vec2 f = fract(p);
    f = f * f * (3.0 - 2.0 * f);
    
    return mix(
        mix(hash(i),              hash(i + vec2(1.0, 0.0)), f.x),
        mix(hash(i + vec2(0.0, 1.0)), hash(i + vec2(1.0, 1.0)), f.x),
        f.y
    );
}

// Zufällige Rotation pro Textur-Kachel
vec2 randomRotateUV(vec2 uv) {
    vec2 grid = floor(uv);
    vec2 local = fract(uv) - 0.5;
    
    float angle = fract(sin(dot(grid, vec2(127.1, 311.7))) * 43758.5453) * 6.2832;
    
    float s = sin(angle);
    float c = cos(angle);
    local = vec2(c * local.x - s * local.y, s * local.x + c * local.y);
    
    return local + 0.5;
}

void vertex() {
    world_pos = (MODEL_MATRIX * vec4(VERTEX, 1.0)).xyz;
    world_normal = normalize((MODEL_MATRIX * vec4(NORMAL, 0.0)).xyz);
}

void fragment() {
    vec2 world_uv = world_pos.xz * texture_scale;
    
    // Texturen samplen mit zufälliger Rotation
    vec4 grass  = texture(grass_texture,  randomRotateUV(world_uv));
    vec4 forest = texture(forest_texture, randomRotateUV(world_uv * 1.1));
    vec4 rock   = texture(rock_texture,   randomRotateUV(world_uv * 0.9));
    
    float height = world_pos.y;
    float slope = 1.0 - world_normal.y;
    
    // Gras/Waldboden Übergang per smoothem Noise → keine Streifen
    float grass_forest_blend = smoothstep(0.3, 0.7,
        smoothNoise(world_pos.xz * 0.05)
    );
    vec4 ground = mix(grass, forest, grass_forest_blend);
    
    // Fels ab rock_height und bei steilen Hängen
    float rock_blend = smoothstep(rock_height - blend_range, rock_height + blend_range, height);
    rock_blend = max(rock_blend, smoothstep(0.4, 0.7, slope));
    
    ALBEDO = mix(ground, rock, rock_blend).rgb;
    ROUGHNESS = mix(0.8, 1.0, rock_blend);
}