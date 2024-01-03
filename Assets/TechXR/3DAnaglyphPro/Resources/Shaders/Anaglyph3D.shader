Shader "Hidden/Anaglyph3D"
{
    Properties
    {
        _MainTex  ("Left channel",  2D) = "white" {}
        _MainTex2 ("Right channel", 2D) = "white" {}
    }
    SubShader
    {
        Cull Off ZWrite Off ZTest Always

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #pragma shader_feature ANAGLYPH_SRGB_CONVERT

            #include "UnityCG.cginc"

            uniform sampler2D _MainTex, _MainTex2;

            // Represents the 6x3 matrix, as an array.
            uniform float _AnaglyphMatrix[6 * 3];

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

        #ifdef ANAGLYPH_SRGB_CONVERT
            fixed4 srgbToLinear(fixed4 x)
            {
                fixed4 result;

                if (x.r <= 0.0031308) { result.r = x.r * 12.92; }
                else { result.r = 1.055 * pow(x.r, 1.0 / 2.4) - 0.055; }

                if (x.g <= 0.0031308) { result.g = x.g * 12.92; }
                else { result.g = 1.055 * pow(x.g, 1.0 / 2.4) - 0.055; }

                if (x.b <= 0.0031308) { result.b = x.b * 12.92; }
                else { result.b = 1.055 * pow(x.b, 1.0 / 2.4) - 0.055; }

                result.a = x.a;

                return result;
            }

            fixed4 linearToSrgb(fixed4 x)
            {
                fixed4 result;

                if (x.r <= 0.04045) { result.r = x.r / 12.92; }
                else { result.r = pow((x.r + 0.055) / 1.055, 2.4); }

                if (x.g <= 0.04045) { result.g = x.g / 12.92; }
                else { result.g = pow((x.g + 0.055) / 1.055, 2.4); }

                if (x.b <= 0.04045) { result.b = x.b / 12.92; }
                else { result.b = pow((x.b + 0.055) / 1.055, 2.4); }

                result.a = x.a;

                return result;
            }
        #endif

            fixed4 frag(v2f i) : SV_Target
            {
                // Sample textures
                fixed4 left  = tex2D(_MainTex,  i.uv);
                fixed4 right = tex2D(_MainTex2, i.uv);

                // Convert SRGB to linear space
            #if !defined(UNITY_COLORSPACE_GAMMA) && defined(ANAGLYPH_SRGB_CONVERT)
                left = srgbToLinear(left);
                right = srgbToLinear(right);
            #endif

                // Apply matrices
                fixed4 result;
                result.r = left.r  * _AnaglyphMatrix[6 * 0 + 0] +
                           left.g  * _AnaglyphMatrix[6 * 0 + 1] +
                           left.b  * _AnaglyphMatrix[6 * 0 + 2] +
                           right.r * _AnaglyphMatrix[6 * 0 + 3] +
                           right.g * _AnaglyphMatrix[6 * 0 + 4] +
                           right.b * _AnaglyphMatrix[6 * 0 + 5];
                result.g = left.r  * _AnaglyphMatrix[6 * 1 + 0] +
                           left.g  * _AnaglyphMatrix[6 * 1 + 1] +
                           left.b  * _AnaglyphMatrix[6 * 1 + 2] +
                           right.r * _AnaglyphMatrix[6 * 1 + 3] +
                           right.g * _AnaglyphMatrix[6 * 1 + 4] +
                           right.b * _AnaglyphMatrix[6 * 1 + 5];
                result.b = left.r  * _AnaglyphMatrix[6 * 2 + 0] +
                           left.g  * _AnaglyphMatrix[6 * 2 + 1] +
                           left.b  * _AnaglyphMatrix[6 * 2 + 2] +
                           right.r * _AnaglyphMatrix[6 * 2 + 3] +
                           right.g * _AnaglyphMatrix[6 * 2 + 4] +
                           right.b * _AnaglyphMatrix[6 * 2 + 5];
                result.a = left.a + right.a;

            #if !defined(UNITY_COLORSPACE_GAMMA) && defined(ANAGLYPH_SRGB_CONVERT)
                return linearToSrgb(result);
            #else
                return result;
            #endif
            }
            ENDCG
        }
    }
}
