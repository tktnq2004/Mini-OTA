using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web;

namespace Hotel.Models
{
	public class GenQRCode
	{
        private string bankCode = "MB"; // Mã ngân hàng MB Bank
        private string accountNumber = "00696919112004"; // Số tài khoản nhận tiền
        private string amount;
        private string message;
        public string QRCode(decimal tongTien,int BookingID) 
		{
            this.amount = tongTien.ToString("F0", CultureInfo.InvariantCulture);
            this.message = $"Thanh toán hóa đơn {BookingID}"; // Nội dung thanh toán

            // Tạo URL QR Code VietQR
            return $"https://img.vietqr.io/image/{bankCode}-{accountNumber}-compact.png?amount={amount}&addInfo={message}";

        }
    }
}