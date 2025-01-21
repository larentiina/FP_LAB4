module PlatformBehaviour

open SFML.Window
open SFML.Graphics
open SFML.System
open SFML.Audio
open GameConsts
open GameState

let drawPlatforms (window: RenderWindow) =
    platforms |> List.iter (fun platform ->
        let platformShape = RectangleShape(Size = Vector2f(platform.Width, platform.Height), Position = platform.Position)
        platformShape.FillColor <- Color.Black
        window.Draw(platformShape)
    )