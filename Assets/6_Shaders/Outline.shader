Shader "Sprites/Outline"
{
    Properties
    {
        _MainTex ("Sprite Texture", 2D) = "white" {}
        _OutlineColor ("OutlineColor", Color) = (1,1,1,1)
        _OutlineSize ("OutlineSize", Range(0.0, 0.1)) = 0.1
    }

    SubShader
    {
        Tags
        {
            "Queue"="Transparent"
        }

        Blend SrcAlpha OneMinusSrcAlpha

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
            float4 _MainTex_ST;
            float4 _MainTex_TexelSize;
         
            fixed4 _OutlineColor;
            float _OutlineSize;
         
            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                return o;
            }
         
            fixed4 frag (v2f i) : SV_Target
            {
                fixed4 col = tex2D(_MainTex, i.uv);


                if (_OutlineSize != 0)
                {
                    fixed leftPixel = tex2D(_MainTex, i.uv + float2(-_MainTex_TexelSize.x - _OutlineSize, 0)).a;
                    fixed upPixel = tex2D(_MainTex, i.uv + float2(0, _MainTex_TexelSize.y + _OutlineSize)).a;
                    fixed rightPixel = tex2D(_MainTex, i.uv + float2(_MainTex_TexelSize.x + _OutlineSize, 0)).a;
                    fixed bottomPixel = tex2D(_MainTex, i.uv + float2(0, -_MainTex_TexelSize.y - _OutlineSize)).a;
                    
                    fixed outline = (1 - leftPixel * upPixel * rightPixel * bottomPixel) * col.a;
                    
                    return lerp(col, _OutlineColor, outline);
                }
                
                else
                {
                    return col;
                }
            }
            ENDCG
        }
    }
}