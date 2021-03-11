//--------------------------------------------------------------
//              Sunao Shader ShadowCaster
//                      Copyright (c) 2021 揚茄子研究所
//--------------------------------------------------------------


	#include "UnityCG.cginc"

struct VIN {
	float4 vertex : POSITION;
	float2 uv     : TEXCOORD0;
};


struct VOUT {
	V2F_SHADOW_CASTER;
	float2 uv : TEXCOORD1;
};


VOUT vert (VIN v) {
	VOUT o;
	o.pos = UnityObjectToClipPos(v.vertex);
	o.uv = v.uv;
	TRANSFER_SHADOW_CASTER(o)

	return o;
}


float4 frag (VOUT IN) : COLOR {
	SHADOW_CASTER_FRAGMENT(IN)
}
