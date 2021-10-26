Shader "Unlit/CardDestroy"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _Color ("Texture", Color) = (1.0, 1.0, 1.0, 1.0)
        _Value ("Value", Range(0.0, 1.0)) = 0
    }
    SubShader
    {
        Blend SrcAlpha OneMinusSrcAlpha
        
        Tags {"RenderType"="Opaque" }
        LOD 100

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };

            sampler2D _MainTex;
            float4 _Color;
            float _Value;
            float4 _MainTex_ST;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                fixed4 col = _Color;
                float4 texcol = tex2D(_MainTex, i.uv);
                
                if (_Value >= 0)
                {
                    _Value = lerp(_Value, 1, _Time.y * 0.1);
                }
                if (col.a <= 1)
                {
                    col.r = lerp(col.r, 0, _Time.y * 0.1);
                    col.g = lerp(col.g, 0, _Time.y * 0.1);
                    col.b = lerp(col.b, 0, _Time.y * 0.1);
                }

                clip(texcol - _Value);
                
                return col * texcol;
            }
            ENDCG
        }
    }
}
