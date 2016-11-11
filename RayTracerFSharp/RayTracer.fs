module RayTracerFSharp.Tracer

open GlmNet
open Tracer

let nonZero (vector:vec3) = vector.x <> 0.0f || vector.y <> 0.0f || vector.z <> 0.0f

let zeroVector = new vec3(0.0f, 0.0f, 0.0f)

let Trace (rayorig:vec3) (raydir:vec3) (depth:int) (sceneObjects:ResizeArray<SceneObject>) =

    let lights = sceneObjects |> Seq.filter (fun o -> o.GetMaterial(zeroVector).emissive |> nonZero)
    
    sceneObjects
    |> Seq.filter (fun sceneObject -> sceneObject.Intersects(rayorig, raydir))
    |> Seq.head
    |> (fun sceneObject -> 
        let distance = sceneObject.IntersectDistance(rayorig, raydir)
        let pointOfIntersection = rayorig + (raydir * distance)

        let material = sceneObject.GetMaterial(pointOfIntersection)
        let normal = sceneObject.GetSurfaceNormal(pointOfIntersection)

        let intensity = lights |> Seq.sumBy ( fun lightSource ->
            let lightDirection = lightSource.GetRayFrom(pointOfIntersection)
            let lightIntensity = glm.dot(normal, lightDirection)
            if lightIntensity < 0.0f then 0.0f else lightIntensity)

        material.albedo * intensity)