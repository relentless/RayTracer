using System;
using System.Linq;
using System.Collections.Generic;
using GlmNet;
using Tracer;

namespace RayTracerCSharp
{
    public static class RayTracer {

        private static vec3 ZERO_VECTOR = new vec3(0, 0, 0);

        private static bool Zero(vec3 vector) {
            return vector.x == 0 && vector.y == 0 && vector.z == 0;
        }

        // Trace a ray into the scene, return the accumulated light value
        public static vec3 TraceRay(vec3 rayorig, vec3 raydir, int depth, List<SceneObject> sceneObjects) {

            var lights = sceneObjects.Where(o => !Zero(o.GetMaterial(ZERO_VECTOR).emissive));

            foreach (var sceneObject in sceneObjects) {

                if (sceneObject.Intersects(rayorig, raydir)) {

                    float distance = sceneObject.IntersectDistance(rayorig, raydir);
                    var pointOfIntersection = rayorig + (raydir * distance);

                    var material = sceneObject.GetMaterial(pointOfIntersection);
                    var normal = sceneObject.GetSurfaceNormal(pointOfIntersection);

                    var intensity = lights.Sum(lightSource => {
                        var lightDirection = lightSource.GetRayFrom(pointOfIntersection);
                        var lightIntensity = glm.dot(normal, lightDirection);
                        return lightIntensity < 0 ? 0 : lightIntensity;
                    });

                    var pixelColour = material.albedo * intensity;
                    return pixelColour;
                }
            }

            throw new Exception("Ray didn't hit anything");
        }
    }
}
