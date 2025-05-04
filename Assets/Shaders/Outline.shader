Shader "Custom/Perfect2DOutline"
{
    Properties
    {
        _MainTex("Sprite Texture", 2D) = "white" {}
        _OutlineColor("Outline Color", Color) = (1,1,1,1)
        _OutlineSize("Outline Size", Float) = 1
    }

    SubShader
    {
        Tags { "Queue" = "Transparent" "RenderType" = "Transparent" "PreviewType" = "Sprite" }
        LOD 100

        Cull Off
        Lighting Off
        ZWrite Off
        Blend SrcAlpha OneMinusSrcAlpha

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            sampler2D _MainTex;
            float4 _OutlineColor;
            float _OutlineSize;
            float4 _MainTex_TexelSize;

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

            v2f vert(appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            float sampleAlpha(float2 uv)
            {
                return tex2D(_MainTex, uv).a;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                float2 uv = i.uv;
                float alpha = sampleAlpha(uv);
                float2 offset = _MainTex_TexelSize.xy * _OutlineSize;

                // Если пиксель непрозрачный — рисуем сам спрайт
                if (alpha > 0.1)
                {
                    return tex2D(_MainTex, uv);
                }

                // Проверка альфы вокруг текущего пикселя
                float surrounding =
                    sampleAlpha(uv + float2(-offset.x, 0)) +
                    sampleAlpha(uv + float2(offset.x, 0)) +
                    sampleAlpha(uv + float2(0, -offset.y)) +
                    sampleAlpha(uv + float2(0, offset.y)) +
                    sampleAlpha(uv + float2(-offset.x, -offset.y)) +
                    sampleAlpha(uv + float2(offset.x, -offset.y)) +
                    sampleAlpha(uv + float2(-offset.x, offset.y)) +
                    sampleAlpha(uv + float2(offset.x, offset.y));

                if (surrounding > 0.1)
                {
                    return float4(_OutlineColor.rgb, 1.0);
                }

                return float4(0, 0, 0, 0);
            }
            ENDCG
        }
    }
}
