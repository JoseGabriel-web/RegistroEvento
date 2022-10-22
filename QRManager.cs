using UnityEngine;
using System.Collections;
using System;
using ZXing;
using ZXing.QrCode;
using UnityEngine.UI;


public class QRManager : MonoBehaviour
{
    [SerializeField]
    private RawImage rawImage;

    private Texture2D texture;

    // Scanner variables
    [SerializeField]
    private RawImage scannerRawImage;
    [SerializeField]
    private AspectRatioFitter scannerAspectRatioFitter;
    [SerializeField]
    private Text scannerInfo;
    [SerializeField]
    private RectTransform scannerZone;

    private bool isCamAvailable;
    private WebCamTexture scannerTexture;


    private void Start()
    {
        texture = new Texture2D(256, 256);
    }

    private void Update()
    {
        UpdateCameraRender();
    }

    public Color32 [] GenerateQRCode(string textForEncoding)
    {
        BarcodeWriter writer = new BarcodeWriter
        {
            Format = BarcodeFormat.QR_CODE,
            Options = new QrCodeEncodingOptions
            {
                Height = texture.height,
                Width = texture.width
            }
        };

        return writer.Write(textForEncoding);
    }


    public void DisplayQRCode(string text)
    {
        Color32[] _convertPixelToTexture = GenerateQRCode(text);
        texture.SetPixels32(_convertPixelToTexture);
        texture.Apply();

        rawImage.texture = texture;
    }


    // scanner functions

    private void SetUpCamera()
    {
        WebCamDevice[] devices = WebCamTexture.devices;

        if (devices.Length == 0)
        {
            isCamAvailable = false;
            return;
        }

        for (int i = 0; i < devices.Length; i++)
        {
            if (devices[i].isFrontFacing == false)
            {
                scannerTexture = new WebCamTexture(devices[i].name, (int)scannerZone.rect.width, (int)scannerZone.rect.height);
            }
        }

        scannerTexture.Play();
        scannerRawImage.texture = scannerTexture;
        isCamAvailable = true;
    }

    private void Scan()
    {
        try
        {
            IBarcodeReader barcodereader = new BarcodeReader();
            Result result = barcodereader.Decode(scannerTexture.GetPixels32(), scannerTexture.width, scannerTexture.height);

            if(result != null)
            {
                scannerInfo.text = result.Text;
            } else
            {
                scannerInfo.text = "Fallo al leer";
            }
        } catch
        {
            scannerInfo.text = "fallo tratando.";
        }
    }

    private void UpdateCameraRender()
    {
        if(isCamAvailable == false)
        {
            return;
        }
        float ratio = (float)scannerTexture.width / (float)scannerTexture.height;
        scannerAspectRatioFitter.aspectRatio = ratio;

        int orientation = scannerTexture.videoRotationAngle;
        scannerRawImage.rectTransform.localEulerAngles = new Vector3(0, 0, orientation);
    }

}