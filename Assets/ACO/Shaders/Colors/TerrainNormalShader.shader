Shader "ACO/TerrainNormalShader"
{
    Properties
    {
        _Angle("Angle", float) = 0.5
        _Width("MaxAngleRange", float) = 0.5
        _Color("ColorNormal", Color) = (0,1.0,1.0,1.0)
        _Color2("ColorRange", Color) = (0,1.0,1.0,1.0)
        _Color3("ColorNoAngle", Color) = (0,1.0,1.0,1.0)
    }

    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 100
        Pass
        {
            Cull Back

            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"
            struct VERTEX_IN
            {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
            };
            struct VERTEX_OUT
            {
                float4 vertex : SV_POSITION;
                float3 normal : NORMAL;
            };

            float _Width;
            float _Angle;
            float4 _Color;
            float4 _Color2;
            float4 _Color3;

            VERTEX_OUT vert (VERTEX_IN v)
            {
                VERTEX_OUT o=(VERTEX_OUT)0;

                o.vertex=mul(unity_ObjectToWorld, float4(v.vertex.xyz, 1.0));

                o.vertex=mul(UNITY_MATRIX_V, o.vertex);
                o.vertex=mul(UNITY_MATRIX_P, o.vertex);

                o.normal = mul((float3x3)unity_ObjectToWorld, v.normal);

                return o;
            }
            fixed4 frag (VERTEX_OUT i) : SV_Target
            {               
                float difference = acos(dot(i.normal, float3(0,1,0))) - _Angle*0.0174533;

                float4 color;

                if(difference > _Width*0.0174533)
                {
                    color = _Color3;
                }
                else if(difference>0)
                {
                    color = _Color2;
                }
                else
                {
                    color = _Color;
                }

                
                return float4(color);
            }
            ENDCG
        }
    }
}