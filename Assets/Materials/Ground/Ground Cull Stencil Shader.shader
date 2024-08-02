Shader "Custom/GroundStencilShader"
{
    SubShader
    {
        // Use any pass that does not render visible content
        Pass
        {
            // Set stencil properties
            Stencil
            {
                Ref 1 // Reference value for the stencil test
                Comp Always // Comparison function
                Pass Replace // Operation to perform when the stencil test passes
            }

            // Use a simple shader that writes nothing to the color buffer
            ZWrite Off
            ColorMask 0
            
            Tags {"Queue" = "Geometry-5"}
        }
    }

    // Disable shadow casting and receiving
    Fallback Off
}
