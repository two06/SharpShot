using System;
using System.Runtime.InteropServices;
using System.Drawing;
using System.Windows.Forms;
using System.Collections.Generic;
using System.IO;

namespace SharpShot
{
    class Program
    {
        static void Main(string[] args)
        {
            printHeader();
            //parse the args into commands.
            var arguments = new Dictionary<string, string>();
            foreach(var argument in args)
            {
                var idx = argument.IndexOf(':');
                if(idx > 0)
                {
                    arguments[argument.Substring(0, idx).Remove(0,1)] = argument.Substring(idx + 1);
                }
                else
                {
                    arguments[argument.Remove(0,1)] = string.Empty;
                }
            }

            if (arguments.ContainsKey("help"))
            {
                printUsage();
                return;
            }
            //
            if (arguments["outformat"] == "img")
            {
                if (!arguments.ContainsKey("outfolder"))
                {
                    Console.WriteLine("[*] /outfolder is required when outputting as an image!");
                    return;
                }

                try
                {
                    var fileName = Path.GetRandomFileName();
                    var filePath = arguments["outfolder"] + "\\" + fileName;
                    if (arguments.ContainsKey("native"))
                    {
                        Console.WriteLine("[*] Taking Screenshot using Windows API...");
                        var image = CaptureWindow(User32.GetDesktopWindow());
                        image.Save(filePath, System.Drawing.Imaging.ImageFormat.Png);
                    }
                    else
                    {
                        Console.WriteLine("[*] Taking screenshot using .NET...");
                        var image = CaptureWindowCLR();
                        image.Save(filePath, System.Drawing.Imaging.ImageFormat.Png);
                    }
                    Console.WriteLine("[*] Screenshot saved to " + filePath);
                }
                catch (Exception)
                {
                    Console.WriteLine("[*] Exception when taking screenshot!");
                }
                
                return;
            }
            else if (arguments["outformat"] == "base64")
            {
                Console.WriteLine("[*] Base64 selected...");
                try
                {
                    if (arguments.ContainsKey("native"))
                    {
                        Console.WriteLine("[*] Taking screenshot using Windows API...");
                        var image = CaptureWindow(User32.GetDesktopWindow());
                        var base64 = GetCompressedImage(image);
                        Console.WriteLine(base64);
                    }
                    else
                    {
                        Console.WriteLine("[*] Taking screenshot using .NET...");
                        var bitmap = CaptureWindowCLR();
                        var base64 = GetCompressedImage(bitmap);
                        Console.WriteLine(base64);
                    }
                }
                catch (Exception)
                {
                    Console.WriteLine("[*] Exception when taking screenshot!");
                }
                Console.WriteLine("[*] Decode to recover image.");
                return;
            }
            else
            {
                printUsage();
                return;
            }
            
        }

        private static string GetCompressedImage(Image image)
        {
            using (var stream = new MemoryStream())
            {
                image.Save(stream, System.Drawing.Imaging.ImageFormat.Png);
                var bytes = stream.ToArray();
                var base64 = Convert.ToBase64String(bytes);
                return base64;
            }
        }

        private static Bitmap CaptureWindowCLR()
        {
            var bmp = new Bitmap(Screen.PrimaryScreen.Bounds.Width, Screen.PrimaryScreen.Bounds.Height);
            using (Graphics g = Graphics.FromImage(bmp))
            {
                g.CopyFromScreen(0, 0, 0, 0, Screen.PrimaryScreen.Bounds.Size);
                return bmp;
            }
        }




        private static Image CaptureWindow(IntPtr handle)
        {
            //get the hDC of the target window
            IntPtr hdcSrc = User32.GetWindowDC(handle);
            //calculate the size
            User32.RECT windowRECT = new User32.RECT();
            User32.GetWindowRect(handle, ref windowRECT);
            int width = windowRECT.right - windowRECT.left;
            int height = windowRECT.bottom - windowRECT.top;

            //create the device context
            IntPtr hdcDest = GDI32.CreateCompatibleDC(hdcSrc);
            //create a bitmap
            IntPtr hBitmap = GDI32.CreateCompatibleBitmap(hdcSrc, width, height);
            IntPtr hOld = GDI32.SelectObject(hdcDest, hBitmap);
            GDI32.BitBlt(hdcDest, 0, 0, width, height, hdcSrc, 0, 0, GDI32.SRCCOPY);

            GDI32.SelectObject(hdcDest, hOld);

            //Create a .NET bitmap we can use elsehwere 
            var img = Image.FromHbitmap(hBitmap);

            //clean up
            GDI32.DeleteDC(hdcDest);
            User32.ReleaseDC(handle, hdcSrc);
            GDI32.DeleteObject(hBitmap);

            return img;
        }

        private static void printHeader()
        {
            Console.WriteLine(@" _____ _                      _____ _           _   ");
            Console.WriteLine(@"/  ___| |                    /  ___| |         | |  ");
            Console.WriteLine(@"\ `--.| |__   __ _ _ __ _ __ \ `--.| |__   ___ | |_ ");
            Console.WriteLine(@" `--. \ '_ \ / _` | '__| '_ \ `--. \ '_ \ / _ \| __|");
            Console.WriteLine(@"/\__/ / | | | (_| | |  | |_) /\__/ / | | | (_) | |_ ");
            Console.WriteLine(@"\____/|_| |_|\__,_|_|  | .__/\____/|_| |_|\___/ \__|");
            Console.WriteLine(@"                       | |                          ");
            Console.WriteLine(@"                       |_|                          ");
            Console.WriteLine("");
            Console.WriteLine("By @two06");
            Console.WriteLine("");
            Console.WriteLine("run SharpShot.exe /help for usage instructions");
        }

        private static void printUsage()
        {
            Console.WriteLine("Capture a full windows screenshot and save to disk using the Windows API:");
            Console.WriteLine(@"    SharpShot.exe /outfolder:""c:\windows\temp"" /outformat:img /native");
            Console.WriteLine("");
            Console.WriteLine("Capture a full window screenshot and save to disk, using .NET methods:");
            Console.WriteLine(@"    SharpShot.exe /outfolder:""c:\windows\temp"" /outformat:img");
            Console.WriteLine("");
            Console.WriteLine("Capture a full window screenshot and write to the console as gzip compressed base64 using the Windows API:");
            Console.WriteLine(@"    SharpShot.exe /outfolder:""c:\windows\temp"" /outformat:img /native");
            Console.WriteLine("");
            Console.WriteLine("Capture a full window screenshot and write to the console as gzip compressed base64 using .NET methods:");
            Console.WriteLine(@"    SharpShot.exe /outfolder:""c:\windows\temp"" /outformat:img");
        }
    }
}
