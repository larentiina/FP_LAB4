module Game
open SFML.Window
open SFML.Graphics
open SFML.System
open GameState
open GameConsts

// Функция для обработки столкновения с монетами
let checkCoinCollision (position: Vector2f) (squareSize: float32) (coins: Coin list) =
    let newCoins, collectedCoins =
        coins |> List.fold (fun (remaining, collected) coin ->
            let coinBounds = FloatRect(coin.Position.X, coin.Position.Y, coin.Size, coin.Size)
            
            // Проверка, пересекается ли квадрат с монетой (по любой стороне квадрата)
            let squareBounds = FloatRect(position.X, position.Y, squareSize, squareSize)
            
            if coinBounds.Intersects(squareBounds) then
                (remaining, coin::collected)  // Добавляем монету в список собранных
            else
                (coin::remaining, collected)  // Оставляем монету в списке оставшихся
        ) ([], [])

    newCoins, List.length collectedCoins  // Возвращаем обновленный список монет и количество собранных монет

//доработать
let isGrounded (position: Vector2f) (platforms: Platform list) =
    platforms
    |> List.exists (fun platform ->
        let platformShape = RectangleShape(Size = Vector2f(platform.Width, platformHeight), Position = platform.Position)
        let platformBounds = platformShape.GetGlobalBounds()

        // Проверка, совпадает ли нижняя граница квадрата с верхней границей платформы
        let isTouchingPlatform = 
            (position.Y + squareSize = platform.Position.Y || platformBounds.Contains(position.X + squareSize / 2.0f, position.Y + squareSize))&&
            position.X + squareSize / 2.0f >= platform.Position.X && 
            position.X + squareSize / 2.0f <= platform.Position.X + platform.Width

        isTouchingPlatform
    )
// Функция для обработки логики прыжка
let updatePosition (position: Vector2f) (verticalSpeed: float32) (isJumping: bool) (horizontalShift: float32) =
    let newVerticalSpeed = 
        if isJumping then verticalSpeed + gravity 
        else 0.0f
    let newPosition = 
        if isJumping then Vector2f(position.X + horizontalShift, position.Y + newVerticalSpeed)
        else Vector2f(position.X + horizontalShift, position.Y)
    
    // Проверка, стоит ли квадрат на платформе или земле
    let grounded = isGrounded newPosition platforms

    if grounded then 
        let clampedY = min newPosition.Y (float32 windowHeight - squareSize)
        (Vector2f(newPosition.X, clampedY), 0.0f, false)
    else 
        (newPosition, newVerticalSpeed, true)

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
        playerBounds.Intersects(enemyBounds)  // Если прямоугольники пересекаются, значит, произошло столкновение
    )
// Функция для отрисовки монет
let drawCoins (window: RenderWindow) (coins: Coin list) =
    coins |> List.iter (fun coin ->
        let coinShape = RectangleShape(Size = Vector2f(coin.Size, coin.Size), FillColor = Color.Yellow)
        coinShape.Position <- coin.Position
        window.Draw(coinShape)
    )

let drawPlatforms (window: RenderWindow) =
    platforms |> List.iter (fun platform ->
                let platformShape = RectangleShape(Size = Vector2f(platform.Width, platformHeight), Position = platform.Position)
                platformShape.FillColor <- Color.Green
                window.Draw(platformShape)
            )

// Рекурсивная функция игрового цикла
let rec gameLoop (state: GameState) =
    if window.IsOpen then
        // Обработка событий
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
                CollectedCoins=state.CollectedCoins }
            gameLoop newState
    
        else
            // Обновляем позицию
            let (newPosition, newVerticalSpeed, newIsJumping) = 
                updatePosition state.Position state.VerticalSpeed state.isJumping horizontalShift

            if checkEnemyCollision newPosition squareSize enemies then
                printfn "Game Over! You hit an enemy!"
                window.Close()

            // Проверка столкновения с монетами
            let remainingCoins, collected = checkCoinCollision newPosition squareSize state.Coins
            let newCollectedCoins = state.CollectedCoins + collected
            // Отрисовка
            window.Clear(Color.White)

            
            drawPlatforms window
            drawCoins window remainingCoins
            drawEnemies window enemies

            let square = RectangleShape(Size = Vector2f(squareSize, squareSize), FillColor = Color.Red)
            square.Position <- newPosition
            window.Draw(square)
            window.Display()
            
            let newState = {
                Position = newPosition
                VerticalSpeed = newVerticalSpeed
                isJumping = newIsJumping
                Coins = remainingCoins
                CollectedCoins=newCollectedCoins }
            gameLoop newState