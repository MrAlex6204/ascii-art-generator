using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.IO;

namespace ASCII_ArtGenerator {
    class Program {
        static void Main(string[] args) {

            Run();

        }

        private static void Run() {


            MainTitle((chr) => {

                Console.ResetColor();
                if ("║═╔╗╚╝".Contains(chr)) {
                    Console.ForegroundColor = ConsoleColor.White;
                } else if ("█".Contains(chr)) {
                    Console.ForegroundColor = ConsoleColor.Green;
                }
                Console.Write(chr);

            });

            var image = Ask("Type a valid image file path : ", (imagePath) => {

                var file = new FileInfo(imagePath);
                var bExists = file.Exists;

                if (!bExists) {

                    Console.ForegroundColor = ConsoleColor.Yellow;
                    Console.WriteLine(" [ THE SPECIFIED FILE DOES NOT EXIST ]");
                    Console.ReadLine();

                } else if (!".jpg,.png,.tiff,.gif".Contains(file.Extension)) {
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    Console.WriteLine(" [ INVALID FILE EXTENSION ]");
                    Console.ReadLine();

                    return false;
                }


                return bExists;
            }, ConsoleColor.Green);

            GenerateAsciiImage(image);

            if (Continue("Continue generating images [Y/N] : ", new[] { ConsoleKey.Y, ConsoleKey.N }, ConsoleColor.Yellow) == ConsoleKey.Y) {

                Console.Clear();//Clear screen and run again
                Run();

            }
        }

        private static string Ask(string text, Func<string, bool> isValid, ConsoleColor answerColor = ConsoleColor.White) {
            var (startX, startY) = (Console.CursorLeft, Console.CursorTop);
            string value = "";

            Console.ResetColor();
            Console.Write(text);

            Console.ForegroundColor = answerColor;

            value = Console.ReadLine();



            if (!isValid(value)) {
                var (endX, endY) = (Console.CursorLeft, Console.CursorTop);

                for (int y = startY; y <= endY; y++) {//Clear screen bounds
                    for (int x = 0; x < 80; x++) {
                        Console.SetCursorPosition(x, y);
                        Console.Write(" ");
                    }
                }

                Console.SetCursorPosition(startX, startY);
                return Ask(text, isValid, answerColor);
            }

            Console.ResetColor();

            return value;
        }

        private static ConsoleKey Continue(string text, ConsoleKey[] opts, ConsoleColor answerColor = ConsoleColor.White) {
            var (startX, startY) = (Console.CursorLeft, Console.CursorTop);
            ConsoleKey keyPressed;

            Console.ResetColor();
            Console.Write(text);

            Console.ForegroundColor = answerColor;
            keyPressed = Console.ReadKey().Key;

            if (!opts.Contains(keyPressed)) {
                Console.ResetColor();
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine();
                Console.WriteLine("[ INVALID OPTION ]");
                Console.ReadKey();

                var (endX, endY) = (Console.CursorLeft, Console.CursorTop);

                for (int y = startY; y <= endY; y++) {//Clear screen bounds
                    for (int x = 0; x < 80; x++) {
                        Console.SetCursorPosition(x, y);
                        Console.Write(" ");
                    }
                }

                Console.SetCursorPosition(startX, startY);
                return Continue(text, opts, answerColor);
            }

            Console.ResetColor();

            return keyPressed;
        }

        private static void GenerateAsciiImage_Method2(string imagePath) {
            Bitmap image = new Bitmap(imagePath);
            int width = image.Width / 3;
            int height = image.Height / 3;
            Bitmap resizedImage = new Bitmap(image, new Size(width, height));
            string asciiArt = "";
            string asciiChars = "@%#*+=-:. ";
            float[,] errorDiffusion = {
            { 0, 0, 7 },
            { 3, 5, 1 }
        };
            for (int y = 0; y < height - 1; y++) {
                for (int x = 0; x < width - 1; x++) {
                    Color pixel = resizedImage.GetPixel(x, y);
                    int brightness = (int)((pixel.R + pixel.G + pixel.B) / 3);
                    int index = (int)(brightness / (255.0f / (asciiChars.Length - 1)));
                    asciiArt += asciiChars[index];
                    int error = brightness - (index * (255 / (asciiChars.Length - 1)));
                    for (int i = 0; i < errorDiffusion.GetLength(0); i++) {
                        for (int j = 0; j < errorDiffusion.GetLength(1); j++) {
                            int errorX = x + i - 1;
                            int errorY = y + j - 1;
                            if (errorX >= 0 && errorX < width && errorY >= 0 && errorY < height) {
                                Color errorPixel = resizedImage.GetPixel(errorX, errorY);
                                int errorBrightness = (int)((errorPixel.R + errorPixel.G + errorPixel.B) / 3);
                                int newErrorBrightness = errorBrightness + (int)(error * (errorDiffusion[i, j] / 16.0f));
                                newErrorBrightness = Math.Max(Math.Min(newErrorBrightness, 255), 0);
                                Color newErrorPixel = Color.FromArgb(newErrorBrightness, newErrorBrightness, newErrorBrightness);
                                resizedImage.SetPixel(errorX, errorY, newErrorPixel);
                            }
                        }
                    }
                }
                asciiArt += "\n";
                //Console.WriteLine(asciiArt);
            }

            Console.WriteLine("IMAGE :");
            Console.WriteLine(asciiArt);
            Console.WriteLine("\n\n\n");
            Console.ReadKey();
        }

        private static void GenerateAsciiImage(string imagePath) {
            Bitmap image = new Bitmap(imagePath);

            int width = Console.BufferWidth - 5;
            int height = (int)(width / ((double)image.Width / image.Height));

            Bitmap resizedImage = new Bitmap(image, new Size(width, height));
            StringBuilder asciiArt = new StringBuilder();

            string asciiChars = "@%#*+=-:. ";
            ConsoleColor[] colors = new[] {
                ConsoleColor.Black,
                ConsoleColor.DarkGray,
                ConsoleColor.Gray,
                ConsoleColor.White
            };

            for (int y = 0; y < height; y++) {

                for (int x = 0; x < width; x++) {

                    Color pixel = resizedImage.GetPixel(x, y);

                    int brightness = (int)((pixel.R + pixel.G + pixel.B) / 3);
                    int index = (int)(brightness / (255.0f / (asciiChars.Length - 1)));

                    asciiArt.Append(asciiChars[index]);
                }

                asciiArt.Append("\n");
            }
            Console.WriteLine(asciiArt.ToString());

            Console.WriteLine("\n\n\n");
        }

        private static void MainTitle(Action<char> thisChar = null) {

            Console.ResetColor();
            Console.WriteLine("\n\n");
            string[] title = {

                "   █████╗ ███████╗ ██████╗██╗██╗     ██████╗ ███████╗███╗   ██╗███████╗██████╗  █████╗ ████████╗ ██████╗ ██████╗ ",
                "  ██╔══██╗██╔════╝██╔════╝██║██║    ██╔════╝ ██╔════╝████╗  ██║██╔════╝██╔══██╗██╔══██╗╚══██╔══╝██╔═══██╗██╔══██╗",
                "  ███████║███████╗██║     ██║██║    ██║  ███╗█████╗  ██╔██╗ ██║█████╗  ██████╔╝███████║   ██║   ██║   ██║██████╔╝",
                "  ██╔══██║╚════██║██║     ██║██║    ██║   ██║██╔══╝  ██║╚██╗██║██╔══╝  ██╔══██╗██╔══██║   ██║   ██║   ██║██╔══██╗",
                "  ██║  ██║███████║╚██████╗██║██║    ╚██████╔╝███████╗██║ ╚████║███████╗██║  ██║██║  ██║   ██║   ╚██████╔╝██║  ██║",
                "  ╚═╝  ╚═╝╚══════╝ ╚═════╝╚═╝╚═╝     ╚═════╝ ╚══════╝╚═╝  ╚═══╝╚══════╝╚═╝  ╚═╝╚═╝  ╚═╝   ╚═╝    ╚═════╝ ╚═╝  ╚═╝",
            };

            foreach (var line in title) {

                foreach (var chr in line.ToCharArray()) {

                    if (thisChar != null) {
                        thisChar(chr);
                    } else {
                        Console.Write(chr);
                    }

                }
                Console.WriteLine();

            }

            Console.ResetColor();
            Console.WriteLine("\n\n");

        }

    }
}
