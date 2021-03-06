﻿// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

//Yves Wang @ FISH, 2015, All rights reserved
Shader "ParticlePhysics2D/MeshLineRender"
{
	Properties
	{
		_Color ("Tint", Color) = (1,1,1,1)
		
	}

	SubShader
	{
		Tags
		{ 
			"Queue"="Transparent" 
			"IgnoreProjector"="True" 
			"RenderType"="Transparent" 
			"PreviewType"="Plane"
			"ForceNoShadowCasting" = "True"
		}

		Cull Off
		Lighting Off
		ZWrite Off
		ZTest Always
		fog {mode off}
		//Blend One OneMinusSrcAlpha
		Blend SrcAlpha OneMinusSrcAlpha
		
		Pass
		{
		CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#include "UnityCG.cginc"
			
			struct appdata_t
			{
				float4 vertex   : POSITION;
				//float4 color    : COLOR;
				//float2 texcoord : TEXCOORD0;
			};

			struct v2f
			{
				float4 vertex   : SV_POSITION;
				//fixed4 color    : COLOR;
				//float2 texcoord  : TEXCOORD0;
			};
			
			uniform fixed4 _Color;

			v2f vert(appdata_t IN)
			{
				v2f OUT;
				//float4 v = float4(IN.vertex,0);
				OUT.vertex = UnityObjectToClipPos(IN.vertex);
				//OUT.vertex = IN.texcoord;
				//OUT.texcoord = IN.texcoord;
				//OUT.color = IN.color * _Color;
				return OUT;
			}


			fixed4 frag(v2f IN) : SV_Target
			{
//				fixed4 c = _Color;
//				c.rgb *= c.a;
//				return c;
				return _Color;
			}
		ENDCG
		}
	}
}

