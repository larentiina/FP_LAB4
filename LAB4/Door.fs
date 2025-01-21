module DoorBehaviour

open SFML.Window
open SFML.Graphics
open SFML.System
open SFML.Audio
open GameConsts
open GameState

let checkDoorCollisison (position: Vector2f) (squareSize: float32) (door: RectangleShape) =
    let doorBounds = door.GetGlobalBounds() 
    let squareRightX = position.X + squareSize

    squareRightX >= doorBounds.Left && 
    squareRightX <= doorBounds.Left + 1.0f &&
    position.Y = door.Position.Y + squareSize