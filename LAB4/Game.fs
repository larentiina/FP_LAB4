module Game
open SFML.Window
open SFML.Graphics
open SFML.System
open SFML.Audio
open GameState
open GameConsts
open System.Threading


let checkCoinCollision (position: Vector2f) (squareSize: float32) (coins: Coin list) =
    let newCoins, collectedCoins =
        coins |> List.fold (fun (remaining, collected) coin ->
            let coinBounds = FloatRect(coin.Position.X, coin.Position.Y,  coinSize,  coinSize)
            
            let squareBounds = FloatRect(position.X , position.Y, squareSize, squareSize)
            
            if coinBounds.Intersects(squareBounds) then
                coinSound.Play()
                (remaining, coin::collected)  
            else
                (coin::remaining, collected)  
        ) ([], [])

    newCoins, List.length collectedCoins  

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

let drawEnemies (window: RenderWindow) (enemies: Enemy list) =
    enemies |> List.iter (fun enemy ->
        let enemyShape = RectangleShape(Size = Vector2f(enemy.Size, enemy.Size), FillColor = Color.Magenta)
        enemyShape.Position <- enemy.Position
        window.Draw(enemyShape)
    )

let checkEnemyCollision (position: Vector2f) (squareSize: float32) (enemies: Enemy list) =
    enemies |> List.exists (fun enemy ->
        let playerBounds = FloatRect(position.X, position.Y, squareSize, squareSize)
        let enemyBounds = FloatRect(enemy.Position.X, enemy.Position.Y, enemy.Size, enemy.Size)
        playerBounds.Intersects(enemyBounds)  
    )

let drawCoins (window: RenderWindow) (coins: Coin list) =
    coins |> List.iter (fun coin ->
        let coinShape = RectangleShape(Size = Vector2f(coinSize,  coinSize), FillColor = Color.Yellow)
        coinShape.Position <- coin.Position
        window.Draw(coinShape)
    )

let coinCounter (coins: int) =
    let label = Text(sprintf "Coins Left: %d" coins, font)
    label.Position <- coinCounterPosition
    label.CharacterSize <- 30u  
    label.FillColor <- Color.Red
    label

let finishText = 
    let winText = Text("You Win!", font, 60u) 
    winText.FillColor <- Color.Green
    winText.Position <- Vector2f(float32 windowWidth / 2.0f - 100.0f, float32 windowHeight / 2.0f - 50.0f) 
    printf "%f" stopwatch.Elapsed.TotalMinutes
    let timetText = Text(sprintf "Time: %.2f min" stopwatch.Elapsed.TotalMinutes, font, 50u)
    timetText.FillColor <- Color.Green
    timetText.Position <- Vector2f(float32 windowWidth / 2.0f - 120.0f, float32 windowHeight / 2.0f) // Центр окна
    window.Clear(Color.White)
    window.Draw(winText)
    window.Draw(timetText)
    window.Display()

let drawPlatforms (window: RenderWindow) =
    platforms |> List.iter (fun platform ->
        let platformShape = RectangleShape(Size = Vector2f(platform.Width, platform.Height), Position = platform.Position)
        platformShape.FillColor <- Color.Black
        window.Draw(platformShape)
    )

let checkDoorCollisison (position: Vector2f) (squareSize: float32) (door: RectangleShape) =
    let doorBounds = door.GetGlobalBounds() 
    let squareRightX = position.X + squareSize

    squareRightX >= doorBounds.Left && 
    squareRightX <= doorBounds.Left + 1.0f &&
    position.Y = door.Position.Y + squareSize


let moveEnemies (enemies: Enemy list) (platforms: Platform list) =
    enemies |> List.map (fun enemy ->
        let newPosition = Vector2f(enemy.Position.X + enemy.Direction * enemy.Speed, enemy.Position.Y)
        
        let isAtPlatformEdge =
            platforms |> List.exists (fun platform ->
                let enemyBounds = FloatRect(newPosition.X, newPosition.Y, enemy.Size, enemy.Size)
                let platformBounds = FloatRect(platform.Position.X, platform.Position.Y, platform.Width, platform.Height)
                
                enemyBounds.Intersects(platformBounds) &&
                (newPosition.X <= platform.Position.X ||
                 newPosition.X + enemy.Size >= platform.Position.X + platform.Width) 
            )
        
        if isAtPlatformEdge then
            { enemy with Direction = -enemy.Direction }
        else
            { enemy with Position = newPosition }
    )

let rec gameLoop (state: GameState) =
    if window.IsOpen then
        window.DispatchEvents()
        let isMovingRight = Keyboard.IsKeyPressed(Keyboard.Key.Right)
        let isMovingLeft = Keyboard.IsKeyPressed(Keyboard.Key.Left)
        let horizontalShift =
            if isMovingLeft then -moveSpeed
            elif isMovingRight then moveSpeed
            else 0.0f
        if Keyboard.IsKeyPressed(Keyboard.Key.Up) && not state.isJumping then
            let newState = {
                Position = state.Position
                VerticalSpeed = jumpHeight
                isJumping = true
                Coins = state.Coins
                CollectedCoins = state.CollectedCoins
                Enemies = state.Enemies 
            }
            gameLoop newState
    
        else
            let (newPosition, newVerticalSpeed, newIsJumping) = 
                updatePosition state.Position state.VerticalSpeed state.isJumping horizontalShift
          
            let updatedEnemies = moveEnemies state.Enemies platforms
            
            if checkEnemyCollision newPosition squareSize updatedEnemies then
                damageSound.Play()
                gameLoop initalState  

            let remainingCoins, collected = checkCoinCollision newPosition squareSize state.Coins
            let newCollectedCoins = state.CollectedCoins + collected

            window.Clear(Color.White)

            window.Draw(coinCounter (finishCoins - newCollectedCoins))
            drawPlatforms window
            drawCoins window remainingCoins
            drawEnemies window updatedEnemies

            let doorShape = RectangleShape(Size = door.Size,Position = door.Position)
            if state.CollectedCoins = finishCoins then
                doorShape.FillColor<-Color.Green
            else
                doorShape.FillColor<-Color.Red
            window.Draw(doorShape)

            let square = RectangleShape(Size = Vector2f(squareSize, squareSize), FillColor = Color.Red)
            square.Position <- newPosition
            window.Draw(square)
            window.Display()

            if checkDoorCollisison newPosition squareSize doorShape && doorShape.FillColor = Color.Green then
                stopwatch.Stop()
                finishText
                Thread.Sleep(3000)
                window.Close()
  
            let newState = {
                Position = newPosition
                VerticalSpeed = newVerticalSpeed
                isJumping = newIsJumping
                Coins = remainingCoins
                CollectedCoins = newCollectedCoins
                Enemies = updatedEnemies 
            }
            gameLoop newState
