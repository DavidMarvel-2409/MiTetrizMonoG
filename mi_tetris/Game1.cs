using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Net.Http.Headers;

namespace mi_tetris
{
    public class Game1 : Game
    {
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;
        private KeyboardState _current;
        private KeyboardState _prev;
        private SpriteFont _fuente;

        private Texture2D _happy_texture;
        private Texture2D _0o0_texture;
        private Texture2D _wall_texture;
        private Texture2D _duda_texture;

        private Random _random;
        private bool is_rotate;
        private bool is_respawn;
        private bool is_bloqued;
        private Vector2 origen;

        private int w_an;
        private int h_an;

        private int[][] map;
        private Vector2 dimentions;
        private int line_index;
        private int[][] clone_map;

        private int[][] TE_up = new int[][] { new int[] { 1, 1, 1 }, new int[] { 0, 1, 0 } };
        private int[][] TE_down = new int[][] { new int[] { 0, 1, 0 }, new int[] { 1, 1, 1 } };
        private int[][] TE_left = new int[][] { new int[] { 1, 0 }, new int[] { 1, 1 }, new int[] { 1, 0 } };
        private int[][] TE_right = new int[][] { new int[] { 0, 1 }, new int[] { 1, 1 }, new int[] { 0, 1 } };

        private int[][] EL_up = new int[][] { new int[] { 1, 0 }, new int[] { 1, 0 }, new int[] { 1, 1 } };
        private int[][] EL_down = new int[][] { new int[] { 1, 1 }, new int[] { 0, 1 }, new int[] { 0, 1 } };
        private int[][] EL_left = new int[][] { new int[] { 0, 0, 1 }, new int[] { 1, 1, 1 } };
        private int[][] EL_right = new int[][] { new int[] { 1, 1, 1 }, new int[] { 1, 0, 0 } };

        private int[][] JO_up = new int[][] { new int[] { 0, 1 }, new int[] { 0, 1 }, new int[] { 1, 1 } };
        private int[][] JO_down = new int[][] { new int[] { 1, 1 }, new int[] { 1, 0 }, new int[] { 1, 0 } };
        private int[][] JO_left = new int[][] { new int[] { 1, 1, 1 }, new int[] { 0, 0, 1 } };
        private int[][] JO_right = new int[][] { new int[] { 1, 0, 0 }, new int[] { 1, 1, 1 } };

        private int[][] II_up_down = new int[][] { new int[] { 1 }, new int[] { 1 }, new int[] { 1 }, new int[] { 1 } };
        private int[][] II_left_right = new int[][] { new int[] { 1, 1, 1, 1 } };

        private int[][] CU = new int[][] { new int[] { 1, 1 }, new int[] { 1, 1 } };

        private int[][] ES_up_down = new int[][] { new int[] { 0, 1, 1 }, new int[] { 1, 1, 0 } };
        private int[][] ES_left_right = new int[][] { new int[] { 1, 0 }, new int[] { 1, 1 }, new int[] { 0, 1 } };

        private int[][] DO_up_down = new int[][] { new int[] { 1, 1, 0 }, new int[] { 0, 1, 1 } };
        private int[][] DO_left_right = new int[][] { new int[] { 0, 1 }, new int[] { 1, 1 }, new int[] { 1, 0 } };

        private bool up;
        private bool left;
        private bool right;
        private bool down;

        private bool game_over;

        private List<Buttons> buttons;
        private Buttons boton;
        private bool savin_butons;

        private class Fig
        {
            public Vector2 coor;
            public int an;
            public int[][] arr;
            public int type;
            public int ori;
            public bool is_move;
            public bool is_rotate;
            public bool is_up;
        }

        private Fig piece;
        private Fig clone;
        private Fig save;

        public Game1()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
        }

        protected override void Initialize()
        {
            redim(720);
            Window.Title = "ACOMODA Y DEJA CAER";
            
            buttons = new List<Buttons>();
            boton = new Buttons();
            savin_butons = false;
            up = false;
            left = false;
            right = false;
            down = false;
            line_index = 0;
            is_rotate = false;
            is_respawn = false;
            game_over = false;
            _random = new Random();

            piece = new Fig();
            //piece.an = GraphicsDevice.Viewport.Width / 45;
            piece.an = GraphicsDevice.Viewport.Width / 35;
            piece.is_move = false;
            piece.is_rotate = false;
            piece.is_up = false;
            piece.arr = TE_up;
            piece.type = _random.Next(1, 8);
            piece.ori = _random.Next(1, 5);

            clone = new Fig();
            clone.an = piece.an;
            clone.type = _random.Next(1, 8);
            clone.ori = _random.Next(1, 5);

            is_bloqued = false;
            int an = (GraphicsDevice.Viewport.Width / piece.an) - 10;
            int al = (GraphicsDevice.Viewport.Height / piece.an) - 2;
            dimentions = new Vector2(an, al);


            map = new int[al][];
            for (int i = 0; i < map.Length; i++)
            {
                map[i] = new int[an];
            }
            origen = new Vector2(map[0].Length / 2, 0);
            piece.coor = origen;

            clone_map = new int[8][];
            for (int i = 0; i < clone_map.Length; i++)
            {
                clone_map[i] = new int[clone_map.Length];
            }

            set_arr(piece);
            set_arr(clone);
            clone.coor = new Vector2(3 - (clone.arr[0].Length / 2), 3 - (clone.arr.Length / 2));
            create_map();
            add_to_map();
            base.Initialize();
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);
            _happy_texture = Content.Load<Texture2D>("feliz");
            _0o0_texture = Content.Load<Texture2D>("0o0");
            _wall_texture = Content.Load<Texture2D>("Brick_Block");
            _duda_texture = Content.Load<Texture2D>("duda");
            _fuente = Content.Load<SpriteFont>("fuente");
        }

        protected override void Update(GameTime gameTime)
        {

            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();
            if (game_over)
            {
                set_game_over();
            }
            else
            {
                //KeyboardState _current = Keyboard.GetState();
                _prev = _current;
                _current = Keyboard.GetState();

                if (is_complete_line())
                {
                    move_all();
                }
                detect_controll();
                move();

                //if(Keyboard.GetState().GetPressedKeys(Keys.R))
                if (((Keyboard.GetState().IsKeyDown(Keys.R) || Keyboard.GetState().IsKeyDown(Keys.RightControl)) && piece.is_rotate == false)
                    || GamePad.GetState(PlayerIndex.One).Buttons.Y == ButtonState.Pressed)
                {
                    rotate();
                    piece.is_rotate = true;
                }
                if (Keyboard.GetState().IsKeyUp(Keys.R) && Keyboard.GetState().IsKeyUp(Keys.RightControl))
                {
                    piece.is_rotate = false;
                }
                if (!Keyboard.GetState().IsKeyDown(Keys.R) && GamePad.GetState(PlayerIndex.One).Buttons.Y == ButtonState.Released)
                {
                    is_rotate = false;
                }
                if (Keyboard.GetState().IsKeyDown(Keys.Q) && !is_respawn)
                {
                    re_spawn();
                    is_respawn = true;
                }
                if (!Keyboard.GetState().IsKeyDown(Keys.Q))
                    is_respawn = false;

                /*if (Keyboard.GetState().IsKeyDown(Keys.E) || GamePad.GetState(PlayerIndex.One).Buttons.RightStick == ButtonState.Pressed)
                {
                    //Console.WriteLine("ele");
                    clone.arr = piece.arr;
                    clone.coor = new Vector2(20 * 35, 10);
                }*/


                if (is_bloqued == true)
                {
                    dead_piece();
                }

                remove_from_map();
                add_to_map();

            }
                
            base.Update(gameTime);
        }
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            Color color = Color.White;
            Rectangle rec1 = new Rectangle(piece.an * 2, piece.an * 2, piece.an, piece.an);
            Rectangle rec2 = new Rectangle((int)dimentions.X * 23, (int)dimentions.Y * 10, clone.an, clone.an);
            Rectangle rec3 = new Rectangle(piece.an * 2, piece.an * 2, piece.an/2, piece.an/2);
            _spriteBatch.Begin();
            for (int i = 0; i < map.Length; i++)
            {
                for (int j = 0; j < map[0].Length; j++)
                {
                    rec1.X = piece.an + piece.an * j;
                    rec1.Y = piece.an + piece.an * i;
                    if (map[i][j] == 1)
                    {
                        _spriteBatch.Draw(_wall_texture, rec1, color);
                    }
                    else if (map[i][j] == 2)
                    {
                        _spriteBatch.Draw(_happy_texture, rec1, color);
                    }
                    else if (map[i][j] == 3)
                    {
                        _spriteBatch.Draw(_0o0_texture, rec1, color);
                    }
                    else if (map[i][j] == 4)
                    {
                        _spriteBatch.Draw(_duda_texture, rec1, color);
                    }
                }
            }
            for(int i = 0; i < clone_map.Length; i++)
            {
                for (int j = 0; j < clone_map[0].Length; j++)
                {
                    rec3.X = (piece.an * (map[0].Length + 1)) + (piece.an/2 * j);
                    rec3.Y = piece.an + piece.an/2 * i;
                    if (clone_map[i][j] == 1)
                    {
                        _spriteBatch.Draw(_wall_texture, rec3, color);
                    }
                    else if (clone_map[i][j] == 2)
                    {
                        _spriteBatch.Draw(_happy_texture, rec3, color);
                    }
                }
            }
            string texto = "Creado por David Valdes Hernandez.(Aun en proceso)";
            float t_scale = (GraphicsDevice.Viewport.Width * 0.4f) / _fuente.MeasureString(texto).X;
            _spriteBatch.DrawString(_fuente, texto, new Vector2(10f / 5f, 10f / 5f), new Color(10f / 255f, 10f / 255f, 102f / 255f), 0f, Vector2.Zero, t_scale, SpriteEffects.None, 1f);

            _spriteBatch.End();

            base.Draw(gameTime);
        }

        private void redim(int H)
        {
            int W = (H * 16) / 9;
            _graphics.PreferredBackBufferHeight = H;
            _graphics.PreferredBackBufferWidth = W;
            //_graphics.ApplyChanges();
            //_graphics.IsFullScreen = true;
            //_graphics.PreferredBackBufferWidth = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width;
            //_graphics.PreferredBackBufferHeight = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height;
            _graphics.ApplyChanges();
        }

        private void move()
        {
            if (piece.is_up)
            {
                Vector2 vv = new Vector2(0, 1);
                if (is_legal(vv)) piece.coor += vv;
                else
                {
                    is_bloqued = true;
                    piece.is_up = false;
                }
            }
            if (!piece.is_move && !piece.is_up)
            {
                Vector2 vv;
                if (up)
                {
                    piece.is_up = true;
                }
                if (down)
                {
                    vv = new Vector2(0, 1);
                    if (is_legal(vv)) piece.coor += vv;
                    else is_bloqued = true;
                }
                if (left)
                {
                    vv = new Vector2(-1, 0);
                    if (is_legal(vv)) piece.coor += vv;
                }
                if (right)
                {
                    vv = new Vector2(1, 0);
                    if (is_legal(vv)) piece.coor += vv;
                }
                piece.is_move = true;
            }
            if (Keyboard.GetState().IsKeyUp(Keys.W) && Keyboard.GetState().IsKeyUp(Keys.Up) &&
                Keyboard.GetState().IsKeyUp(Keys.S) && Keyboard.GetState().IsKeyUp(Keys.Down) &&
                Keyboard.GetState().IsKeyUp(Keys.A) && Keyboard.GetState().IsKeyUp(Keys.Left) &&
                Keyboard.GetState().IsKeyUp(Keys.D) && Keyboard.GetState().IsKeyUp(Keys.Right) &&
                GamePad.GetState(PlayerIndex.One).DPad.Up == ButtonState.Released &&
                GamePad.GetState(PlayerIndex.One).DPad.Down == ButtonState.Released &&
                GamePad.GetState(PlayerIndex.One).DPad.Left == ButtonState.Released &&
                GamePad.GetState(PlayerIndex.One).DPad.Right == ButtonState.Released &&
                Keyboard.GetState().IsKeyUp(Keys.E) &&
                GamePad.GetState(PlayerIndex.One).Buttons.RightStick == ButtonState.Released)
                piece.is_move = false;

        }
        private void detect_controll()
        {
            if (Keyboard.GetState().IsKeyDown(Keys.W) || Keyboard.GetState().IsKeyDown(Keys.Up) ||
                GamePad.GetState(PlayerIndex.One).DPad.Up == ButtonState.Pressed)
            {
                up = true;
            }
            else
            {
                up = false;
            }
            if (Keyboard.GetState().IsKeyDown(Keys.S) || Keyboard.GetState().IsKeyDown(Keys.Down) ||
                GamePad.GetState(PlayerIndex.One).DPad.Down == ButtonState.Pressed)
            {
                down = true;
            }
            else
            {
                down = false;
            }
            if (Keyboard.GetState().IsKeyDown(Keys.A) || Keyboard.GetState().IsKeyDown(Keys.Left) ||
                GamePad.GetState(PlayerIndex.One).DPad.Left == ButtonState.Pressed)
            {
                left = true;
            }
            else
            {
                left = false;
            }
            if (Keyboard.GetState().IsKeyDown(Keys.D) || Keyboard.GetState().IsKeyDown(Keys.Right) ||
                GamePad.GetState(PlayerIndex.One).DPad.Right == ButtonState.Pressed)
            {
                right = true;
            }
            else
            {
                right = false;
            }
        }

        private void set_game_over()
        {
            for (int i = 0; i < map.Length; i++)
            {
                for (int j = 0; j < map[0].Length; j++)
                {
                    if (map[i][j] != 1 && map[i][j] != 0 && map[i][j] != 4)
                    {
                        map[i][j] = 1;
                    }
                    if (map[i][j] == 4)
                    {
                        map[i][j] = 0;
                    }
                }
            }
        }
        private void move_all()
        {
            for (int i = line_index; i > 0; i--)
            {
                for (int j = 0; j < map[0].Length; j++)
                {
                    if (map[i - 1][j] != 1)
                    {
                        map[i][j] = map[i - 1][j];
                        map[i - 1][j] = 0;
                    }
                }
            }
        }
        private void re_spawn()
        {
            piece.type = clone.type;
            clone.type = _random.Next(1, 8);
            piece.ori = clone.ori;
            clone.ori = _random.Next(1, 5);
            piece.coor = origen;
            set_arr(piece);
            set_arr(clone);
            if (!is_legal(new Vector2(0, 0)))
            {
                game_over = true;
            }
        }

        private bool is_legal(Vector2 mov)
        {
            Vector2 ya_mov = piece.coor + mov;
            int _X = (int)ya_mov.X;
            int _Y = (int)ya_mov.Y;
            for (int i = 0; i < piece.arr.Length; i++)
            {
                for (int j = 0; j < piece.arr[0].Length; j++)
                {
                    if (piece.arr[i][j] == 1)
                    {
                        int checkX = _X + j;
                        int checkY = _Y + i;
                        if (checkY < 0 || checkY >= map.Length || checkX < 0 || checkX >= map[0].Length)
                        {
                            return false;
                        }
                        if (map[checkY][checkX] == 1 || map[checkY][checkX] == 3)
                        {
                            return false;
                        }
                    }
                }
            }
            return true;
        }
        private void dead_piece()
        {
            for (int i = 0; i < map.Length; i++)
            {
                for (int j = 0; j < map[0].Length; j++)
                {
                    if (map[i][j] == 2)
                    {
                        map[i][j] = 3;
                    }
                }
            }
            re_spawn();
            is_bloqued = false;
        }

        private void rotate()
        {

            switch (piece.ori)
            {
                case 1:     //up
                    piece.ori = 3;
                    break;
                case 2:     //down
                    piece.ori = 4;
                    break;
                case 3:     //left
                    piece.ori = 2;
                    break;
                case 4:     //right
                    piece.ori = 1;
                    break;
            }
            set_arr(piece);
            if (!is_legal(new Vector2(0, 0)))
            {
                switch (piece.ori)
                {
                    case 1:     //up
                        piece.ori = 4;
                        break;
                    case 2:     //down
                        piece.ori = 3;
                        break;
                    case 3:     //left
                        piece.ori = 1;
                        break;
                    case 4:     //right
                        piece.ori = 2;
                        break;
                }
                set_arr(piece);
            }
        }

        private void set_arr(Fig fi)
        {
            switch (fi.type)
            {
                case 1:     //TE
                    switch (fi.ori)
                    {
                        case 1:     //UP
                            fi.arr = TE_up;
                            break;
                        case 2:     //DOWN
                            fi.arr = TE_down;
                            break;
                        case 3:     //LEFT
                            fi.arr = TE_left;
                            break;
                        case 4:     //RIGHT
                            fi.arr = TE_right;
                            break;
                    }
                    break;
                case 2:     //EL
                    switch (fi.ori)
                    {
                        case 1:     //UP
                            fi.arr = EL_up;
                            break;
                        case 2:     //DOWN
                            fi.arr = EL_down;
                            break;
                        case 3:     //LEFT
                            fi.arr = EL_left;
                            break;
                        case 4:     //RIGHT
                            fi.arr = EL_right;
                            break;
                    }
                    break;
                case 3:     //JO
                    switch (fi.ori)
                    {
                        case 1:     //UP
                            fi.arr = JO_up;
                            break;
                        case 2:     //DOWN
                            fi.arr = JO_down;
                            break;
                        case 3:     //LEFT
                            fi.arr = JO_left;
                            break;
                        case 4:     //RIGHT
                            fi.arr = JO_right;
                            break;
                    }
                    break;
                case 4:     //II
                    switch (fi.ori)
                    {
                        case 1:     //UP_DOWN
                            fi.arr = II_left_right;
                            break;
                        case 2:     //LEFT_RIGHT
                            fi.arr = II_left_right;
                            break;
                        case 3:     //UP_DOWN
                            fi.arr = II_up_down;
                            break;
                        case 4:     //LEFT_RIGHT
                            fi.arr = II_up_down;
                            break;
                    }
                    break;
                case 5:     //CU
                    fi.arr = CU;
                    break;
                case 6:     //ES
                    switch (fi.ori)
                    {
                        case 1:     //UP_DOWN
                            fi.arr = ES_left_right;
                            break;
                        case 2:     //LEFT_RIGHT
                            fi.arr = ES_left_right;
                            break;
                        case 3:     //UP_DOWN
                            fi.arr = ES_up_down;
                            break;
                        case 4:     //LEFT_RIGHT
                            fi.arr = ES_up_down;
                            break;
                    }
                    break;
                case 7:     //DO
                    switch (fi.ori)
                    {
                        case 1:     //UP_DOWN
                            fi.arr = DO_left_right;
                            break;
                        case 2:     //LEFT_RIGHT
                            fi.arr = DO_left_right;
                            break;
                        case 3:     //UP_DOWN
                            fi.arr = DO_up_down;
                            break;
                        case 4:     //LEFT_RIGHT
                            fi.arr = DO_up_down;
                            break;
                    }
                    break;
            }
        }

        private void create_map()
        {
            for (int i = 0; i < map.Length; i++)
            {
                for (int j = 0; j < map[0].Length; j++)
                {
                    if (j == 0 || j == map[0].Length - 1)
                    {
                        map[i][j] = 1;
                    }
                    else
                    {
                        if (i == map.Length - 1)
                        {
                            map[i][j] = 1;
                        }
                        else
                        {
                            map[i][j] = 0;
                        }
                    }
                }
            }
            for (int i = 0; i < clone_map.Length; i++)
            {
                for (int j = 0; j < clone_map[0].Length; j++)
                {
                    if (i > 0 && i < clone_map[0].Length - 1 && j > 0 && j < clone_map.Length - 1)
                    {
                        clone_map[i][j] = 0;
                    }
                    else
                    {
                        clone_map[i][j] = 1;
                    }
                }
            }
        }
        private void add_to_map()
        {
            for (int i = 0; i < piece.arr.Length; i++)
            {
                for (int j = 0; j < piece.arr[0].Length; j++)
                {
                    if (piece.arr[i][j] == 1)
                    {
                        map[(int)piece.coor.Y + i][(int)piece.coor.X + j] = 2;
                    }
                }
            }
            for (int i = 0; i < clone.arr.Length; i++)
            {
                for (int j = 0; j < clone.arr[0].Length; j++)
                {
                    if (clone.arr[i][j] == 1)
                    {
                        clone_map[(int)clone.coor.Y + i][(int)clone.coor.X + j] = 2;
                    }
                }
            }

            projected_fig();
        }
        private void projected_fig()
        {
            for (int i = 0; i < map.Length; i++)
            {
                for (int j = 0; j < map[0].Length; j++)
                {
                    if (i > piece.coor.Y + piece.arr.Length - 1 && j >= piece.coor.X && j < piece.coor.X + piece.arr[0].Length)
                    {
                        if (map[i][j] == 0)
                            map[i][j] = 4;
                    }
                }
            }
        }
        private void remove_from_map()
        {
            for (int i = 0; i < map.Length; i++)
            {
                for (int j = 0; j < map[0].Length; j++)
                {
                    if (map[i][j] == 2 || map[i][j] == 4)
                    {
                        map[i][j] = 0;
                    }
                }
            }
            for(int i = 0; i < clone_map.Length; i++)
            {
                for (int j = 0; j < clone_map[0].Length; j++)
                {
                    if (clone_map[i][j] == 2)
                    {
                        clone_map[i][j] = 0;
                    }
                }
            }
        }
        private bool is_complete_line()
        {
            for (int i = 0; i < map.Length; i++)
            {
                int Line_counter = 0;
                for (int j = 0; j < map[0].Length; j++)
                {
                    if (map[i][j] == 3) Line_counter++;
                }
                if (Line_counter == map[i].Length - 2)
                {
                    line_index = i;
                    return true;
                }
            }
            return false;
        }
    }
}
