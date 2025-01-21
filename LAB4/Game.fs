module Game

open SFML.Window
open SFML.Graphics
open SFML.System
open SFML.Audio
open System.Threading

open GameState
open GameConsts
open CoinBehaviour
open DoorBehaviour
open EnemyBehaviour
open PlatformBehaviour
open CharacterBehaviour

let clock = Clock() 

let finishText = 
    let winText = Text("You Win!", font, 60u) 
    winText.FillColor <- Color.Green
    winText.Position <- Vector2f(float32 windowWidth / 2.0f - 100.0f, float32 windowHeight / 2.0f - 50.0f) 
    let timetText = Text(sprintf "Time: %.2f min" stopwatch.Elapsed.TotalMinutes, font, 50u)
    timetText.FillColor <- Color.Green
    timetText.Position <- Vector2f(float32 windowWidth / 2.0f - 120.0f, float32 windowHeight / 2.0f) // Центр окна
    window.Clear(Color.White)
    window.Draw(winText)
    window.Draw(timetText)
    window.Display()

let rec gameLoop (state: GameState) =
    if window.IsOpen then
        let deltaTime = clock.Restart().AsSeconds()

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
                CoinAnimation = state.CoinAnimation
            }
            gameLoop newState
    
        else
            let (newPosition, newVerticalSpeed, newIsJumping) = 
                updatePosition state.Position state.VerticalSpeed state.isJumping horizontalShift

            let updatedCoinAnimation = updateCoinAnimation state.CoinAnimation deltaTime
            let updatedEnemies = moveEnemies state.Enemies platforms
            
            if checkEnemyCollision newPosition squareSize updatedEnemies then
                damageSound.Play()
                gameLoop initalState  

            let remainingCoins, collected = checkCoinCollision newPosition squareSize state.Coins
            let newCollectedCoins = state.CollectedCoins + collected

            window.Clear(Color.White)   

            window.Draw(coinCounter (finishCoins - newCollectedCoins))
            drawPlatforms window
            drawCoins window remainingCoins state.CoinAnimation deltaTime
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
                CoinAnimation = updatedCoinAnimation 
            }
            gameLoop newState
