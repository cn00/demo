Shader "chess/Board"
{
    Properties
    {
//        _N ("N", int) = 10 // set by c#
        _LineColor ("LineColor", Color) = (1,0,0,1)
        _MainColor ("MainColor", Color) = (1,0,0,1)
        _MainTex ("Texture", 2D) = "white" {}
    }
    SubShader
    {
        // No culling or depth
        Cull Off ZWrite Off ZTest Always

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

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            int _N = 10;
            float4 _LineColor;
            float4 _MainColor;
            sampler2D _MainTex;

            fixed4 frag (v2f i) : SV_Target
            {
                 float pi = 3.1415926;
                 fixed4 col = tex2D(_MainTex, i.uv) + _MainColor;
                if (abs(sin(_N*i.uv.x*pi)) < 0.1 ||abs(sin(_N*i.uv.y*pi)) < 0.1 ){
                    col = _LineColor;
                }

                // just invert the colors
                // col.rgb = 1 - col.rgb;
                return col;
            }
            ENDCG
        }
    }
}