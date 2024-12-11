shader_type canvas_item;
render_mode blend_add;


void vertex() {
	// Called for every vertex the material is visible on.
}

#define EPS 1E-6
#define TAU 6.283185307179586
#define TAUR 2.5066282746310002
#define SQRT2 1.4142135623730951

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

		float uSize = 1.0;
			// center of the beam segment
        vec2 xy = UV - vec2(0, 0.5);



        float alpha;
        float len = 1.0;

        float sigma = uSize/4.0;
        if (len < EPS) {
        // If the beam segment is too short, just calculate intensity at the position.
            alpha = exp(-pow(length(xy),2.0)/(2.0*sigma*sigma))/2.0/sqrt(uSize);
        } else {
        // Otherwise, use analytical integral for accumulated intensity.
            alpha = erf(xy.x/SQRT2/sigma) - erf((xy.x-len)/SQRT2/sigma);
            alpha *= exp(-xy.y*xy.y/(2.0*sigma*sigma))/2.0/len*uSize;
        }

        float afterglow = .33;
        alpha *= afterglow * 1.;
        COLOR = vec4(0,1,0,alpha);
}

//void light() {
//	// Called for every pixel for every light affecting the CanvasItem.
//	// Uncomment to replace the default light processing function with this one.
//}
