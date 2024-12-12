#[compute]
#version 450


// Invocations in the (x, y, z) dimension
layout (local_size_x = 8, local_size_y = 8, local_size_z = 1) in;

layout (rgba16f, set = 0, binding = 0) uniform image2D color_image;

// storage buffer for the oscilloscope data
layout (std430, set = 0, binding = 1) readonly restrict buffer OscilloscopeData {
    uint data_size; // the number of points
    vec2 data[]; // the points
};

// Our push constant
layout (push_constant, std430) uniform Params {
    vec2 raster_size;
    vec2 reserved;
} params;

float sdSegment(in vec2 p, in vec2 a, in vec2 b)
{
    vec2 pa = p - a, ba = b - a;
    float h = clamp(dot(pa, ba) / dot(ba, ba), 0.0, 1.0);
    return length(pa - ba * h);
}

float sdPoint (in vec2 p, in vec2 a)
{
    return length(p - a);
}

// The code we want to execute in each invocation
void main() {
    ivec2 uv = ivec2(gl_GlobalInvocationID.xy);
    ivec2 size = ivec2(params.raster_size);

    // Prevent reading/writing out of bounds.
    if (uv.x >= size.x || uv.y >= size.y) {
        return;
    }

    vec4 color = imageLoad(color_image, uv);

    //draw white lines

    float minimum = 10000.0;
    for (int i = 1; i < data_size - 2; i++) {
        // skip if distance to point is too far
        float dBetweenPoints = distance(data[i], data[i + 1]);

        if (distance(vec2(uv), data[i]) > dBetweenPoints && distance(vec2(uv), data[i + 1]) > dBetweenPoints) {
            color.b = 1.0;
            continue;
        }

        color.b = 0.0;
        float d = sdSegment(vec2(uv), data[i], data[i + 1]);
        minimum = min(minimum, d);
    }

    color.r = clamp(1.0 - minimum, 0.0, 1.0);

    // line from 0,0 to 10,10
    //color.r = smoothstep(1, 0, sdSegment(vec2(uv), vec2(0.0, 0.0), vec2(100.0, 100.0))/10);
  


    // Write back to our color buffer.
    imageStore(color_image, uv, color);
}