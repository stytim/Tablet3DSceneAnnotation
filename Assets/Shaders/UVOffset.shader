Shader "Custom/UVOffset" {
    Properties {
        _MainTex ("Base (RGB)", 2D) = "white" {}
        _USpeed("USpeed ", float) = 1.0
        _UCount("UCount", float) = 1.0
        _VSpeed("VSpeed", float) = 1.0
        _VCount("VCount", float) = 1.0
    }
    SubShader {
        Tags { "RenderType"="Opaque" }
        LOD 200

        CGPROGRAM
        #pragma surface surf Lambert

        // 贴图
        sampler2D _MainTex;

        // U轴方向滚动速度
        float _USpeed; 
        // U轴方向平铺个数
        float _UCount;

        // V轴方向滚动速度
        float _VSpeed;
        // V轴方向平铺个数
        float _VCount;

        struct Input {
            float2 uv_MainTex;
        };

        void surf (Input IN, inout SurfaceOutput o) {
            float2 uv = IN.uv_MainTex;
            float detalTime = _Time.x;

            //  计算X轴方向变化
            uv.x += detalTime * _USpeed;
            uv.x *=  _UCount;

            // 计算Y轴方向变化
            uv.y += detalTime * _VSpeed;
            uv.y *= _VCount;

            half4 c = tex2D (_MainTex, uv);
            o.Albedo = c.rgb;
            o.Alpha = c.a;
        }
        ENDCG
    } 
    FallBack "Diffuse"
}