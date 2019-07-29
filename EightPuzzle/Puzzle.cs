using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using static EightPuzzle.Constants;

namespace EightPuzzle
{
    class Puzzle
    {
        //Properties.
        public Image[,] Images;
        BitmapImage Img_src;
        List<int> Pool;
        List<int> PoolX;
        List<int> PoolY;
        
        /// <summary>
        /// Return an one-dimension array represent position of puzzles.
        /// </summary>
        /// <returns></returns>
        public List<int> ListOfPosition()
        {
            List<int> pos_list = new List<int>();
            for (int i = 0; i < 3; ++i)
            {
                for (int j = 0; j < 3; ++j)
                {
                    if (Images[j, i] != null)
                        pos_list.Add((int)Images[j, i].Tag);
                    else
                        pos_list.Add(8);
                }
            }
            return pos_list;
        }

        /// <summary>
        /// Puzzle constructor.
        /// </summary>
        /// <param name="source">Path of image.</param>
        /// <param name="pos_list">List of position.</param>
        public Puzzle(string source, List<int> pos_list)
        {
            Images = new Image[3, 3];
            PoolX = new List<int> { 0, 1, 2, 0, 1, 2, 0, 1 };
            PoolY = new List<int> { 0, 0, 0, 1, 1, 1, 2, 2 };

            //Get the selected image from user.
            Img_src = new BitmapImage(new Uri(source));
            PUZZLE_SIZE.WIDTH = (int)PUZZLE_TOTAL_SIZE / 3;
            PUZZLE_SIZE.HEIGHT = (int)(PUZZLE_TOTAL_SIZE * Img_src.Height / Img_src.Width) / 3;

            //Initialize temporary variables.
            CroppedBitmap img_cropped;
            Image[,] temp_imgs = new Image[3, 3];
            Image imgView;
            int index = 0;

            //Initialize image puzzle.
            for (int i = 0; i < 3; ++i)
            {
                for (int j = 0; j < 3; ++j)
                {
                    if (pos_list[index] != 8)
                    {
                        img_cropped = new CroppedBitmap(Img_src, new Int32Rect(
                            (int)(PoolX[pos_list[index]] * Img_src.PixelWidth / 3), (int)(PoolY[pos_list[index]] * Img_src.PixelHeight / 3),
                            (int)Img_src.PixelWidth / 3, (int)Img_src.PixelHeight / 3
                            ));

                        imgView = new Image();
                        imgView.Source = img_cropped;
                        imgView.Tag = pos_list[index];
                        imgView.Height = PUZZLE_SIZE.HEIGHT;
                        imgView.Width = PUZZLE_SIZE.WIDTH;

                        Images[j, i] = imgView;
                    }
                    index++;
                }
            }
        }

        /// <summary>
        /// Puzzle constructor.
        /// </summary>
        /// <param name="source">Path of image.</param>
        public Puzzle(string source)
        {
            //Initialize properties.
            Images = new Image[3, 3];
            Pool = new List<int> { 0, 1, 2, 3, 4, 5, 6, 7 };
            PoolX = new List<int> { 0, 1, 2, 0, 1, 2, 0, 1 };
            PoolY = new List<int> { 0, 0, 0, 1, 1, 1, 2, 2 };

            //Get the selected image from user.
            Img_src = new BitmapImage(new Uri(source));
            PUZZLE_SIZE.WIDTH = (int)PUZZLE_TOTAL_SIZE / 3;
            PUZZLE_SIZE.HEIGHT = (int)(PUZZLE_TOTAL_SIZE * Img_src.Height / Img_src.Width) / 3;

            //Initialize temporary variables.
            CroppedBitmap img_cropped;
            Image imgView;
            var RNG = new Random();
            int randint;

            //Initialize image puzzle.
            for (int i = 0; i < 3; ++i)
            {
                for (int j = 0; j < 3; ++j)
                {
                    if (i != 2 || j != 2)
                    {
                        randint = RNG.Next(Pool.Count);

                        img_cropped = new CroppedBitmap(Img_src, new Int32Rect(
                            (int)(PoolX[randint] * Img_src.PixelWidth / 3), (int)(PoolY[randint] * Img_src.PixelHeight / 3),
                            (int)Img_src.PixelWidth / 3, (int)Img_src.PixelHeight / 3
                            ));

                        imgView = new Image();
                        imgView.Source = img_cropped;
                        imgView.Tag = Pool[randint];
                        imgView.Height = PUZZLE_SIZE.HEIGHT;
                        imgView.Width = PUZZLE_SIZE.WIDTH;

                        Images[j, i] = imgView;

                        Pool.RemoveAt(randint);
                        PoolX.RemoveAt(randint);
                        PoolY.RemoveAt(randint);
                    }
                }
            }
        }
    }
}
