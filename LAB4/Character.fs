module CharacterBehaviour

open SFML.Window
open SFML.Graphics
open SFML.System
open SFML.Audio
open GameConsts
open GameState

let isGrounded (position: Vector2f) (platforms: Platform list) =
    platforms
    |> List.exists (fun platform ->
        let isTouchingPlatform = 
            (platform.Position.Y - (position.Y + squareSize)) = 0.0f && 
            position.X + squareSize / 2.0f >= platform.Position.X && 
            position.X + squareSize / 2.0f <= platform.Position.X + platform.Width

        isTouchingPlatform
    )

let isPlatformLeft (position: Vector2f) (platforms: Platform list) =
    platforms
    |> List.exists (fun platform ->
        let isTouchingPlatformLeft =
            (platform.Position.X = position.X + squareSize - float32 5) && 
            ((position.Y) < (platform.Position.Y + platform.Height)) &&
            ((position.Y) > (platform.Position.Y - squareSize ))
        
        isTouchingPlatformLeft
    )

let isPlatformRight (position: Vector2f) (platforms: Platform list) =
    platforms
    |> List.exists (fun platform ->
        let isTouchingPlatformRight =   
            (platform.Position.X + platform.Width = position.X + float32 5) && 
            (position.Y < (platform.Position.Y + platform.Height)) &&
            (position.Y > (platform.Position.Y - squareSize ))
        
        isTouchingPlatformRight
    )

let isPlatformTop (position: Vector2f) (platforms: Platform list) =
    platforms
    |> List.exists (fun platform ->
        let isTouchingPlatformTop = 
            (platform.Position.Y + platform.Height = (position.Y + float32 4)) &&
            position.X + squareSize / 2.0f >= platform.Position.X && 
            position.X + squareSize / 2.0f <= platform.Position.X + platform.Width
        
        isTouchingPlatformTop
    )

let isInsidePlatform (position: Vector2f) (platforms: Platform list) =
    platforms
    |> List.tryFind (fun platform ->
        position.X + squareSize / 2.0f >= platform.Position.X &&
        position.X + squareSize / 2.0f <= platform.Position.X + platform.Width &&
        position.Y + squareSize >= platform.Position.Y && 
        position.Y < platform.Position.Y                
    )

let updatePosition (position: Vector2f) (verticalSpeed: float32) (isJumping: bool) (horizontalShift: float32) =
    let newVerticalSpeed = 
        if isJumping then verticalSpeed + gravity 
        else 0.0f
    
    let newPosition = 
        Vector2f(position.X + horizontalShift, position.Y + newVerticalSpeed)
    
    let isTouchPlatformLeft = isPlatformLeft newPosition platforms
    let isTouchPlatformRight = isPlatformRight newPosition platforms

    let adjustedHorizontalShift =
        if isTouchPlatformRight && horizontalShift < 0.0f then 0.0f  
        elif isTouchPlatformLeft && horizontalShift > 0.0f then 0.0f  
        else horizontalShift

    let adjustedPosition = Vector2f(position.X + adjustedHorizontalShift, position.Y + newVerticalSpeed)
    let isTouchingPlatformTop = isPlatformTop adjustedPosition platforms

    match isInsidePlatform adjustedPosition platforms with
    | Some platform ->
        let clampedY = platform.Position.Y - squareSize
        (Vector2f(adjustedPosition.X, clampedY), 0.0f, false)
    | None ->
        let newSpeed = if isTouchingPlatformTop then 0.0f else newVerticalSpeed
        let finalPosition = Vector2f(position.X + adjustedHorizontalShift, position.Y + newSpeed)
        let grounded = isGrounded finalPosition platforms

        if grounded then 
            let clampedY = min finalPosition.Y (float32 windowHeight - squareSize)
            (Vector2f(finalPosition.X, clampedY), 0.0f, false)
        else 
            (finalPosition, newSpeed, true)
