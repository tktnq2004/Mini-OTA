using System;
using System.Collections.Generic;
using System.Configuration;
using System.Web;
using VNPAY_CS_ASPX;

public static class VNPayService
{
    public static string CreatePaymentUrl(decimal amount, string orderId, string returnUrl, string ipAddress)
    {
        var vnpay = new VnPayLibrary();

        string vnp_Url = ConfigurationManager.AppSettings["vnp_Url"];
        string vnp_TmnCode = ConfigurationManager.AppSettings["vnp_TmnCode"];
        string vnp_HashSecret = ConfigurationManager.AppSettings["vnp_HashSecret"];

        vnpay.AddRequestData("vnp_Version", "2.1.0");
        vnpay.AddRequestData("vnp_Command", "pay");
        vnpay.AddRequestData("vnp_TmnCode", vnp_TmnCode);
        vnpay.AddRequestData("vnp_Amount", ((long)(amount * 100)).ToString());
        vnpay.AddRequestData("vnp_CurrCode", "VND");
        vnpay.AddRequestData("vnp_TxnRef", orderId);
        vnpay.AddRequestData("vnp_OrderInfo", "Thanh toán đơn hàng #" + orderId);
        vnpay.AddRequestData("vnp_Locale", "vn");
        vnpay.AddRequestData("vnp_ReturnUrl", returnUrl);
        vnpay.AddRequestData("vnp_IpAddr", ipAddress);
        vnpay.AddRequestData("vnp_CreateDate", DateTime.Now.ToString("yyyyMMddHHmmss"));

        string paymentUrl = vnpay.CreateRequestUrl(vnp_Url, vnp_HashSecret);
        return paymentUrl;
    }
}
