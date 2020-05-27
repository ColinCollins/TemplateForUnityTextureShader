Shader "Unlit/NoLightShader"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _Color("Color", Color) = (1, 1, 1, 1)
        _Color2("Color2", Color) = (1, 1, 1, 1)
        _ZSpeed("ZSpeed", float) = 4
    }
    SubShader
    {
        Tags { "RenderType" = "Opaque" }
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
                float zPos : TEXCOORD1;
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;

            float _ZSpeed;
            fixed4 _Color;
            fixed4 _Color2;
            float3 _Points[100];
            float _distance[100];
            fixed zPos;

             float getMin(float datas[100])
            {
                float d = -1;
                for (int i = 0; i < 100; i++)
                {
                    if (d > datas[i] || d < 0)
                        d = datas[i];
                }

                return d;
            }

            fixed4 sampleColor(float3 uvPos) 
            {
                for (int i = 0; i < 100; i++)
                {
                    _distance[i] = distance(uvPos.xyz, _Points[i].xyz);
                }

                return lerp(_Color, _Color2, getMin(_distance));
            } 

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                o.zPos = sin(_Time.y * _ZSpeed);
                
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                fixed4 col = tex2D(_MainTex, i.uv);
                float3 uv = float3(i.uv.x, i.uv.y, i.zPos);

                return sampleColor(uv);
            }

            ENDCG
        }
    }
}
