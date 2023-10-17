// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Custom/AlwaysOnTop" {
	Properties{
		_MainTex("Base", 2D) = "white" {}
		_Color("Color", Color) = (1.0, 1.0, 1.0, 1.0)

		[Toggle] _IsUnlit("is Unlit", Float) = 0
	}

		CGINCLUDE

		#include "UnityCG.cginc"

		sampler2D _MainTex;
		fixed4 _Color;

		half4 _MainTex_ST;

		// unity defined variables
		uniform float4 _LightColor0;
		uniform float _LightInfluence;

		bool _IsUnlit;

		//Magnorama
		float _EnableClipping;
		float _ClipScale = 1;
		float4x4 _WorldToBox;
		float4 _ScaleCenter;

		struct v2f {
			half4 pos : SV_POSITION;
			half2 uv : TEXCOORD0;
			float4 posWorld : TEXCOORD1;
			float3 normalDir : TEXCOORD2;
			fixed4 vertexColor : COLOR;
		};

		v2f vert(appdata_full v) {
			v2f o;

		
			o.uv.xy = TRANSFORM_TEX(v.texcoord, _MainTex);
			o.vertexColor = v.color * _Color;
			o.normalDir = normalize(mul(float4(v.normal, 0.0), unity_WorldToObject));

			o.posWorld = mul(unity_ObjectToWorld, v.vertex);

			if (_EnableClipping > 0.5f)
			{
				float4 vertexPos = mul(unity_ObjectToWorld, v.vertex);
				vertexPos.xyz = _ScaleCenter.xyz + (_ClipScale * (vertexPos.xyz - _ScaleCenter.xyz));
				o.pos = mul(UNITY_MATRIX_VP, vertexPos);
			}
			else
			{
				o.pos = UnityObjectToClipPos(v.vertex);
			}

			return o;
		}

		fixed4 frag(v2f i) : COLOR{

			if (_EnableClipping > 0.5f)
			{
				float3 boxPosition = mul(_WorldToBox, i.posWorld);

				clip(boxPosition + 0.5);
				clip(0.5 - boxPosition);
			}

			// lighting
			float3 lightDirection = normalize(_WorldSpaceLightPos0.xyz);
			float3 diffuseReflection = _LightColor0.xyz * max(0.0, dot(i.normalDir, lightDirection));

			float3 lightFinal = diffuseReflection + UNITY_LIGHTMODEL_AMBIENT;

			if (_IsUnlit == 0)
			{
				return tex2D(_MainTex, i.uv.xy) * float4(_Color.rgb * lightFinal, _Color.a);
			}
			else
			{
				return tex2D(_MainTex, i.uv.xy) * _Color;
			}
			
		}

			ENDCG

			SubShader {
			Tags{ "RenderType" = "Transparent" "Queue" = "Transparent+100" }
				Cull back
				Lighting Off
				ZWrite Off
				ZTest Always
				Fog{ Mode Off }
				Blend SrcAlpha OneMinusSrcAlpha

				Pass{

					CGPROGRAM

					#pragma vertex vert
					#pragma fragment frag
					#pragma fragmentoption ARB_precision_hint_fastest

					ENDCG

			}

		}
		FallBack Off
}