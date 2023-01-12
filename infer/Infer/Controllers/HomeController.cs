using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;
using System.Text;
using Microsoft.AspNetCore.Mvc;
using Infer.Models;
using Infer.Services;

namespace Infer.Controllers;

public class HomeController : Controller
{
    private readonly InferenceService _service;
    private readonly ILogger<HomeController> _log;


    public HomeController(InferenceService service, ILogger<HomeController> log)
    {
        _service = service;
        _log = log;
    }

    public IActionResult Index()
    {
        return View();
    }

    [HttpPost]
    public IActionResult Index(string values)
    {
        if (string.IsNullOrEmpty(values))
            return View();

        var bitmapRaw = GetBitMapFromBase64(values);
        SaveBitMap(bitmapRaw);
        var bitMapProcessed = Process(bitmapRaw, new Size(8, 8));
        var array = GetArray(bitMapProcessed);
        Display(array);
        var list = GetFlattenedArray(array);
        Display(list);
        var digit = _service.Infer(list);
        return View(digit);
    }

    private void Display(long[] list)
    {
        var s = new StringBuilder();
        foreach (var value in list)
        {
            s.Append(value.ToString("D2"));
            s.Append(" ");
        }
        _log.LogInformation(s.ToString());
    }

    private void Display(long[,] bitMapArray)
    {
        var s = new StringBuilder();
        for (var x = 0; x <= 7; x++)
        {
            for (var y = 0; y <= 7; y++)
            {
                var bit = bitMapArray[x,y];
                s.Append(bit.ToString("D2"));
                s.Append(" ");
            }

            s.AppendLine();
        }

        _log.LogInformation(s.ToString());
    }

    private void SaveBitMap(Bitmap image)
    {
        var file = Environment.CurrentDirectory + $"\\images\\{Guid.NewGuid().ToString()[..8]}.bmp";
        _log.LogInformation("Saving {file}");
        image.Save(file, ImageFormat.Bmp);
    }

    private static long[,] GetArray(Bitmap bitmap)
    {
        var list = new long[8, 8];
        for (var x = 0; x <= 7; x++)
        {
            for (var y = 0; y <= 7; y++)
            {
                var bit = bitmap.GetPixel(y,x);
                var scaled = ConvertGrayscaleByteToValue(bit);
                list[x, y] = scaled;
            }
        }

        return list;
    }

    private static int ConvertGrayscaleByteToValue(Color bit)
    {
        // values are inverted, 256 = white
        var value = Math.Abs(256 - bit.R);
        // value is 16 bit but we want 4 bit
        var scaled = value / 16;
        return scaled;
    }

    private static long[] GetFlattenedArray(long[,] list)
    {
        var flat = new List<long>();
        for (var x = 0; x <= 7; x++)
        {
            for (var y = 0; y <= 7; y++)
            {
                flat.Add(list[x, y]);
            }
        }

        return flat.ToArray();
    }

    private static Bitmap GetBitMapFromBase64(string values)
    {
        var base64 = values[22..]; // remove data:image/png;base64
        var bytes = Convert.FromBase64String(base64);
        using var ms = new MemoryStream(bytes);
        var image = Image.FromStream(ms);
        var file = Environment.CurrentDirectory + $"\\images\\{Guid.NewGuid().ToString()[..8]}.png";
        image.Save(file, ImageFormat.Png);
        return new Bitmap(image);
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel {RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier});
    }

    public static Bitmap Process(Bitmap bitmap, Size size)
    {
        return ToGrayscale(ResizeImage(bitmap, size));
    }

    // https://stackoverflow.com/questions/10839358/resize-bitmap-image
    public static Bitmap ResizeImage(Bitmap imgToResize, Size size)
    {
        var b = new Bitmap(size.Width, size.Height);
        using Graphics g = Graphics.FromImage(b);
        g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
        g.DrawImage(imgToResize, 0, 0, size.Width, size.Height);
        return b;
    }

    // https://stackoverflow.com/questions/43891219/convert-32-bit-bitmap-to-8-bit-both-color-and-grayscale
    public static Bitmap ToGrayscale(Bitmap bmp)
    {
        var result = new Bitmap(bmp.Width, bmp.Height, PixelFormat.Format8bppIndexed);

        var resultPalette = result.Palette;

        for (int i = 0; i < 256; i++)
        {
            resultPalette.Entries[i] = Color.FromArgb(255, i, i, i);
        }

        result.Palette = resultPalette;

        BitmapData data = result.LockBits(new Rectangle(0, 0, result.Width, result.Height), ImageLockMode.WriteOnly, PixelFormat.Format8bppIndexed);

        // Copy the bytes from the image into a byte array
        byte[] bytes = new byte[data.Height * data.Stride];
        Marshal.Copy(data.Scan0, bytes, 0, bytes.Length);

        for (int y = 0; y < bmp.Height; y++)
        {
            for (int x = 0; x < bmp.Width; x++)
            {
                var c = bmp.GetPixel(x, y);
                var rgb = (byte) ((c.R + c.G + c.B) / 3);

                bytes[y * data.Stride + x] = rgb;
            }
        }

        // Copy the bytes from the byte array into the image
        Marshal.Copy(bytes, 0, data.Scan0, bytes.Length);

        result.UnlockBits(data);

        return result;
    }
}