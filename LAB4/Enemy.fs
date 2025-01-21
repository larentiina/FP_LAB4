module EnemyBehaviour

open SFML.Window
open SFML.Graphics
open SFML.System
open SFML.Audio
open GameConsts
open GameState

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