// Assets/Shaders/GeomatricShader0.shader
Shader "Custom/GeomatricShader0"
{
    Properties
    {
        _Color ("Color", Color) = (1,1,1,1)
        _PushAmount ("Triangle Push Amount", Float) = 0.1
    }

    SubShader
    {
        Tags { "RenderType"="Opaque" }

        Pass
        {
            HLSLPROGRAM

            #pragma vertex Vert
            #pragma geometry Geo
            #pragma fragment Frag
            #pragma target 4.0

            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
            };

            struct v2g
            {
                float4 clipPos : SV_POSITION;
                float3 objPos  : TEXCOORD0; // object space position
                float3 normal  : NORMAL;
            };

            struct g2f
            {
                float4 clipPos : SV_POSITION;
            };

            float4 _Color;
            float _PushAmount;

            v2g Vert (appdata v)
            {
                v2g o;
                o.clipPos = UnityObjectToClipPos(v.vertex);
                o.objPos  = v.vertex.xyz;
                o.normal  = v.normal;
                return o;
            }

            [maxvertexcount(3)]
            void Geo(
                triangle v2g input[3],
                inout TriangleStream<g2f> triStream
            )
            {
                // Compute triangle normal in object space
                float3 p0 = input[0].objPos;
                float3 p1 = input[1].objPos;
                float3 p2 = input[2].objPos;

                float3 edge1 = p1 - p0;
                float3 edge2 = p2 - p0;

                float3 triNormal = normalize(cross(edge1, edge2));

                g2f o;

                for (int i = 0; i < 3; i++)
                {
                    float3 newObjPos = input[i].objPos + triNormal * _PushAmount;
                    float4 clip = UnityObjectToClipPos(float4(newObjPos, 1.0));
                    o.clipPos = clip;
                    triStream.Append(o);
                }

                triStream.RestartStrip();
            }

            float4 Frag (g2f i) : SV_Target
            {
                return _Color;
            }

            ENDHLSL
        }
    }
}
