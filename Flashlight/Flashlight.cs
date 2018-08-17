using UnityEngine;

namespace Flashlight
{
    class Flashlight
    {
        private static Flashlight singletonInstance;

        public static Flashlight Instance
        {
            get
            {
                singletonInstance = singletonInstance != null ? singletonInstance : new Flashlight();
                return singletonInstance;
            }
        }

        public Light currentLight { get; set; }

        public void SetCurrentFlashlight(ref Light light)
        {
            light.type = LightType.Spot;
            light.color = Color.white;
            light.enabled = currentLight != null ? currentLight.enabled : false;
            light.intensity = 1f;
            light.range = 100f;
            light.spotAngle = 45;
            currentLight = light;
        }
    }
}
