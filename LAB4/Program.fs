open SFML.Window
open SFML.Graphics
open SFML.System

// Параметры экрана
let windowWidth = 800
let windowHeight = 600
let platformHeight = 30.0f

// Создание окна
let window = RenderWindow(VideoMode(uint32 windowWidth, uint32 windowHeight), "Jumping Square")
window.SetFramerateLimit(60u)

// Инициализация квадрата
let squareSize = 50.0f

let platformSize = 150.0f

// Константы для физики
let gravity = 0.5f
let jumpHeight = -10.0f
let moveSpeed = 5.0f
// Тип для монеты
type Coin = {
    Position: Vector2f
    Size: float32
}

type Platform = {
    Position: Vector2f
    Width: float32
}

type Enemy = { Position: Vector2f; Size: float32 }
let platforms = [
    { Position = Vector2f(100.0f, 500.0f); Width = platformSize }
    { Position = Vector2f(300.0f, 450.0f); Width = platformSize }  
    { Position = Vector2f(500.0f, 400.0f); Width = platformSize }  
    { Position =  Vector2f(700.0f, 350.0f); Width = platformSize } 
    { Position =  Vector2f(0.0f, float32 windowHeight - platformHeight); Width = float32 windowWidth }
]

let enemies = [
    { Position = Vector2f(300.0f, 540.0f); Size = 30.0f }
    { Position = Vector2f(610.0f, 370.0f); Size = 30.0f }
    { Position = Vector2f(700.0f, 300.0f); Size = 30.0f }
]

let initialPosition = Vector2f(375.0f, float32 windowHeight - platformHeight - squareSize)// Начальная позиция квадрата

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
// Рекурсивная функция игрового цикла
let rec gameLoop position verticalSpeed isJumping coins collectedCoins =
    if window.IsOpen then
        // Обработка событий
        window.DispatchEvents()
        let isMovingRight = Keyboard.IsKeyPressed(Keyboard.Key.Right)
        let isMovingLeft = Keyboard.IsKeyPressed(Keyboard.Key.Left)
        let horizontalShift =
            if isMovingLeft then -moveSpeed
            elif isMovingRight then moveSpeed
            else 0.0f
        if Keyboard.IsKeyPressed(Keyboard.Key.Up) && not isJumping then
            gameLoop position jumpHeight true coins collectedCoins
    
        else
            // Обновляем позицию
            let (newPosition, newVerticalSpeed, newIsJumping) = 
                updatePosition position verticalSpeed isJumping horizontalShift

            if checkEnemyCollision newPosition squareSize enemies then
                printfn "Game Over! You hit an enemy!"
                window.Close()

            // Проверка столкновения с монетами
            let remainingCoins, collected = checkCoinCollision newPosition squareSize coins
            let newCollectedCoins = collectedCoins + collected
            // Отрисовка
            window.Clear(Color.White)

            platforms |> List.iter (fun platform ->
                let platformShape = RectangleShape(Size = Vector2f(platform.Width, platformHeight), Position = platform.Position)
                platformShape.FillColor <- Color.Green
                window.Draw(platformShape)
            )

            drawCoins window remainingCoins
            drawEnemies window enemies

            let square = RectangleShape(Size = Vector2f(squareSize, squareSize), FillColor = Color.Red)
            square.Position <- newPosition
            window.Draw(square)
            window.Display()
            
            // Следующий шаг игрового цикла
            gameLoop newPosition newVerticalSpeed newIsJumping remainingCoins newCollectedCoins

let initialCoins: Coin list = [
    { Position = Vector2f(120.0f, 460.0f); Size = 20.0f }
    { Position = Vector2f(150.0f, 460.0f); Size = 20.0f }
    { Position = Vector2f(180.0f, 460.0f); Size = 20.0f }
    { Position = Vector2f(500.0f, 270.0f); Size = 20.0f }
    { Position = Vector2f(650.0f, 220.0f); Size = 20.0f }
]



window.Closed.Add(fun _ -> window.Close())
gameLoop initialPosition 0.0f false initialCoins 0
