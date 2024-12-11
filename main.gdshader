shader_type canvas_item;
render_mode blend_add;

#define EPS 1E-6
#define TAU 6.283185307179586
#define TAUR 2.5066282746310002
#define SQRT2 1.4142135623730951



uniform float uSize = 1.0;
uniform float uInvert = 1.0;
uniform float uIntensity = 1.0;
varying vec4 uvl;

void vertex() {
	vec2 pos1 = INSTANCE_CUSTOM.xy;
	vec2 pos2 = INSTANCE_CUSTOM.zw;
    uvl = vec4(0.0);

    vec2 aStart = pos1;
    vec2 aEnd = pos2;

    float aIdx = float(VERTEX_ID);

    float tang;
    vec2 current;
    // All points in quad contain the same data:
    // segment start point and segment end point.
    // We determine point position using its index.
    float idx = mod(aIdx,4.0);

    // `dir` vector is storing the normalized difference
    // between end and start
    vec2 dir = aEnd-aStart;
    uvl.z = length(dir);

    if (uvl.z > EPS) {
        dir = dir / uvl.z;
    } else {
    // If the segment is too short, just draw a square
        dir = vec2(1.0, 0.0);
    }
    // norm stores direction normal to the segment difference
    vec2 norm = vec2(-dir.y, dir.x);

    // `tang` corresponds to shift "forward" or "backward"
    if (idx >= 2.0) {
        current = aEnd;
        tang = 1.0;
        uvl.x = -uSize;
    } else {
        current = aStart;
        tang = -1.0;
        uvl.x = uvl.z + uSize;
    }
    // `side` corresponds to shift to the "right" or "left"
    float side = (mod(idx, 2.0)-0.5)*2.0;
    uvl.y = side * uSize;
    uvl.w = floor(aIdx / 4.0 + 0.5);

    // Calculate the position of the current vertex
    // gl_Position = vec4((current+(tang*dir+norm*side)*uSize)*uInvert,0.0,1.0);
    VERTEX.xy = (MODEL_MATRIX * vec4((current+(tang*dir+norm*side)*uSize)*uInvert,0.0,1.0)).xy;
    //VERTEX = (MODEL_MATRIX * vec4(VERTEX, 0.0, 1.0)).xy;
}


// A standard gaussian function, used for weighting samples
float gaussian(float x, float sigma) {
  return exp(-(x * x) / (2.0 * sigma * sigma)) / (TAUR * sigma);
}

// This approximates the error function, needed for the gaussian integral
float erf(float x) {
  float s = sign(x), a = abs(x);
  x = 1.0 + (0.278393 + (0.230389 + 0.078108 * (a * a)) * a) * a;
  x *= x;
  return s - s / (x * x);
}

void fragment() {
			// center of the beam segment
        vec2 xy =  uvl.xy;



        float alpha;
        float len = uvl.z;

        float sigma = uSize/4.0;
        if (len < EPS) {
        // If the beam segment is too short, just calculate intensity at the position.
            alpha = exp(-pow(length(xy),2.0)/(2.0*sigma*sigma))/2.0/sqrt(uSize);
        } else {
        // Otherwise, use analytical integral for accumulated intensity.
            alpha = erf(xy.x/SQRT2/sigma) - erf((xy.x-len)/SQRT2/sigma);
            alpha *= exp(-xy.y*xy.y/(2.0*sigma*sigma))/2.0/len*uSize;
        }

        float afterglow = 1.0;
        alpha *= afterglow * uIntensity;
        COLOR = vec4(.15,1,.15,alpha);
}

//void light() {
//	// Called for every pixel for every light affecting the CanvasItem.
//	// Uncomment to replace the default light processing function with this one.
//}
