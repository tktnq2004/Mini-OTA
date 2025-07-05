--CREATE DATABASE Hotel
GO
USE Hotel
GO
DROP TABLE IF EXISTS RoomImages;
DROP TABLE IF EXISTS Wishlist;
DROP TABLE IF EXISTS DiscountDetails;
DROP TABLE IF EXISTS Reviews;
DROP TABLE IF EXISTS BookingDetails;
DROP TABLE IF EXISTS Payments;
DROP TABLE IF EXISTS Bookings;	
DROP TABLE IF EXISTS Rooms;
DROP TABLE IF EXISTS Discounts;
DROP TABLE IF EXISTS RoomTypes;
DROP TABLE IF EXISTS Users;	
DROP TABLE IF EXISTS Hotels;
GO
CREATE TABLE Hotels (
    HotelID INT PRIMARY KEY IDENTITY(1,1),
    HotelName NVARCHAR(255) NOT NULL,
    Address NVARCHAR(255) NOT NULL,
	HotelImage NVARCHAR(255) ,
    Latitude DECIMAL(9,6) NOT NULL, 
    Longitude DECIMAL(9,6) NOT NULL, 
);
go
-- Bảng quản lý người dùng
CREATE TABLE Users (
    UserID INT PRIMARY KEY IDENTITY(1,1),
    FullName NVARCHAR(255) NOT NULL,
    Email NVARCHAR(255) UNIQUE NOT NULL,
	Username NVARCHAR(255) NOT NULL,
    Password NVARCHAR(255) NOT NULL,
    Phone NVARCHAR(20),
    Role NVARCHAR(10) DEFAULT 'Customer' CHECK (Role IN ('Admin', 'Customer'))
);
INSERT INTO Users (FullName, Email, Username, Password, Role) 
VALUES ('admin', 'admin@gmail.com', 'admin', 'admin', 'Admin');

CREATE TABLE RoomTypes (
    RoomTypeId INT PRIMARY KEY IDENTITY(1,1),
    RoomTypeName NVARCHAR(100) NOT NULL
);
GO
CREATE TABLE Discounts (
    DiscountID INT PRIMARY KEY IDENTITY(1,1),
    DiscountPercent INT CHECK (DiscountPercent BETWEEN 0 AND 100) NOT NULL,
    StartDate DATE NOT NULL,
    EndDate DATE NOT NULL,
	Images NVARCHAR(MAX)
	);
GO
-- Tạo bảng phòng
CREATE TABLE Rooms (
    RoomID INT PRIMARY KEY IDENTITY(1,1),
    HotelID INT NOT NULL,
    RoomTypeId INT NOT NULL,
    RoomName NVARCHAR(100) NOT NULL,
    Price DECIMAL(10,2) NOT NULL,
    Capacity INT NOT NULL,
    Description NVARCHAR(MAX),
    FOREIGN KEY (HotelID) REFERENCES Hotels(HotelID) ON DELETE CASCADE,
    FOREIGN KEY (RoomTypeId) REFERENCES RoomTypes(RoomTypeId) ON DELETE CASCADE
);
GO
CREATE TABLE DiscountDetails (
    DiscountDetailID INT PRIMARY KEY IDENTITY(1,1),
    DiscountID INT NOT NULL,
    RoomID INT NOT NULL UNIQUE,
    FOREIGN KEY (DiscountID) REFERENCES Discounts(DiscountID) ON DELETE CASCADE,
    FOREIGN KEY (RoomID) REFERENCES Rooms(RoomID) ON DELETE CASCADE
);
-- Tạo bảng ảnh phòng
CREATE TABLE RoomImages (
    ImageID INT PRIMARY KEY IDENTITY(1,1),
    RoomID INT NOT NULL,
    ImageURL NVARCHAR(MAX) NOT NULL,
    FOREIGN KEY (RoomID) REFERENCES Rooms(RoomID) ON DELETE CASCADE
);
GO
-- Tạo bảng giỏ hàng
CREATE TABLE Wishlist (
    WishListID INT PRIMARY KEY IDENTITY(1,1),
    UserID INT NOT NULL,
    RoomID INT NOT NULL,
    FOREIGN KEY (UserID) REFERENCES Users(UserID) ON DELETE CASCADE,
    FOREIGN KEY (RoomID) REFERENCES Rooms(RoomID) ON DELETE CASCADE
);
GO

-- Tạo bảng đặt phòng
CREATE TABLE Bookings (
    BookingID INT PRIMARY KEY IDENTITY(1,1),
    UserID INT NOT NULL,
	RoomID INT NOT NULL,
	Discount INT NOT NULL DEFAULT 0,
    CheckIn DATE NOT NULL,
    CheckOut DATE NOT NULL,
	Amount DECIMAL(10,2) NOT NULL,
    PaymentStatus NVARCHAR(20) DEFAULT 'Pending' CHECK (PaymentStatus IN ('Pending', 'Completed', 'Cancelled')),
    BookingDate DATETIME DEFAULT GETDATE() NOT NULL,
	ExpirationTime DATETIME NOT NULL,
    FOREIGN KEY (UserID) REFERENCES Users(UserID) ON DELETE CASCADE,
	FOREIGN KEY (RoomID) REFERENCES Rooms(RoomID) ON DELETE CASCADE,
);
Go
CREATE TABLE Reviews (
    ReviewID INT PRIMARY KEY IDENTITY(1,1),
    UserID INT NOT NULL,
    RoomID INT NULL,
    HotelID INT NULL,
    Rating INT CHECK (Rating BETWEEN 1 AND 5),
    Comment NVARCHAR(MAX),
    CreatedAt DATETIME DEFAULT GETDATE(),

    FOREIGN KEY (UserID) REFERENCES Users(UserID) ON DELETE CASCADE,
    FOREIGN KEY (RoomID) REFERENCES Rooms(RoomID),       
    FOREIGN KEY (HotelID) REFERENCES Hotels(HotelID),       

    CHECK (
        (RoomID IS NOT NULL AND HotelID IS NULL) OR
        (RoomID IS NULL AND HotelID IS NOT NULL)
    )
);
GO
/*CREATE TABLE BookingDetails (
    BookingDetailID INT PRIMARY KEY IDENTITY(1,1),
    BookingID INT NOT NULL,
    RoomID INT NOT NULL,
	Discount INT NOT NULL DEFAULT 0,
    FOREIGN KEY (BookingID) REFERENCES Bookings(BookingID) ON DELETE CASCADE,
    FOREIGN KEY (RoomID) REFERENCES Rooms(RoomID) ON DELETE CASCADE
);
GO

CREATE TABLE Payments (
	BookingID INT NOT NULL,
    PaymentID INT PRIMARY KEY IDENTITY(1,1),
    
    PaymentDate DATETIME DEFAULT GETDATE() NOT NULL,
	FOREIGN KEY (BookingID) REFERENCES Bookings(BookingID) ON DELETE CASCADE
);
GO*/

INSERT INTO RoomTypes (RoomTypeName) VALUES
('Single'),
('Double'),
('Twin'),
('Standard'),
('Deluxe'),
('Suite');
go
INSERT INTO Hotels (HotelName, HotelImage, Address, Latitude, Longitude)
VALUES
-- Hà Nội
(N'Khách sạn Hà Nội Luxury', N'/Content/hotelimages/hanoi.jpg', N'123 Đường Lê Duẩn, Quận Hoàn Kiếm, Hà Nội', 21.028511, 105.804817),

-- Đà Nẵng
(N'Khách sạn Biển Xanh Đà Nẵng', N'/Content/hotelimages/danang.jpg', N'456 Võ Nguyên Giáp, Quận Sơn Trà, Đà Nẵng', 16.067789, 108.220831),

-- Nha Trang
(N'Khách sạn Nha Trang View', N'/Content/hotelimages/nhatrang.jpg', N'789 Trần Phú, TP. Nha Trang, Khánh Hòa', 12.238791, 109.196749),

-- TP. Hồ Chí Minh
(N'Khách sạn Sài Gòn Central', N'/Content/hotelimages/saigon.jpg', N'321 Nguyễn Huệ, Quận 1, TP. Hồ Chí Minh', 10.776889, 106.700806),

-- Phú Quốc
(N'Khách sạn Biển Ngọc Phú Quốc', N'/Content/hotelimages/phuquoc.jpg', N'01 Trần Hưng Đạo, Dương Đông, Phú Quốc', 10.289900, 103.984000);
GO
INSERT INTO Rooms (HotelID, RoomTypeId, RoomName, Price, Capacity, Description)
VALUES
(1, 6, 'Suite Room Hanoi', 2000000, 4, N'Phòng Suite cao cấp tại Hà Nội.'),
(2, 5, 'Deluxe Room Danang', 1500000, 3, N'Phòng Deluxe sang trọng gần biển Đà Nẵng.'),
(3, 4, 'Standard Room Nha Trang', 900000, 2, N'Phòng tiêu chuẩn view biển Nha Trang.'),
(4, 2, 'Double Room Saigon', 1000000, 2, N'Phòng đôi trung tâm Quận 1, Sài Gòn.'),
(5, 1, 'Single Room Phu Quoc', 800000, 1, N'Phòng đơn yên tĩnh tại Phú Quốc.');

go
-- RoomID 1: Suite Room Hanoi
INSERT INTO RoomImages (RoomID, ImageURL) VALUES
(1, N'/Content/images/1/P101.jpg'),
(1, N'/Content/images/1/P102.jpg'),
(1, N'/Content/images/1/P103.jpg'),
(1, N'/Content/images/1/P104.jpg'),
(1, N'/Content/images/1/P105.jpg');

-- RoomID 2: Deluxe Room Danang
INSERT INTO RoomImages (RoomID, ImageURL) VALUES
(2, N'/Content/images/2/P201.jpg'),
(2, N'/Content/images/2/P202.jpg'),
(2, N'/Content/images/2/P203.jpg'),
(2, N'/Content/images/2/P204.jpg'),
(2, N'/Content/images/2/P205.jpg');

-- RoomID 3: Standard Room Nha Trang
INSERT INTO RoomImages (RoomID, ImageURL) VALUES
(3, N'/Content/images/3/P301.jpg'),
(3, N'/Content/images/3/P302.jpg'),
(3, N'/Content/images/3/P303.jpg'),
(3, N'/Content/images/3/P304.jpg'),
(3, N'/Content/images/3/P305.jpg');
-- RoomID 4: Double Room Saigon
INSERT INTO RoomImages (RoomID, ImageURL) VALUES
(4, N'/Content/images/4/P401.jpg'),
(4, N'/Content/images/4/P402.jpg'),
(4, N'/Content/images/4/P403.jpg'),
(4, N'/Content/images/4/P404.jpg'),
(4, N'/Content/images/4/P405.jpg');

-- RoomID 5: Single Room Phu Quoc
INSERT INTO RoomImages (RoomID, ImageURL) VALUES
(5, N'/Content/images/5/P501.jpg'),
(5, N'/Content/images/5/P502.jpg'),
(5, N'/Content/images/5/P503.jpg'),
(5, N'/Content/images/5/P504.jpg'),
(5, N'/Content/images/5/P505.jpg');
go
INSERT INTO Discounts (DiscountPercent, StartDate, EndDate) 
VALUES 
(10, '2025-06-01', '2025-06-05'),  -- Giảm 10% dịp Quốc tế Thiếu nhi
(25, '2025-09-01', '2025-09-03'),  -- Giảm 25% dịp Quốc khánh 2/9
(20, '2025-12-20', '2025-12-31'),  -- Giảm 20% dịp Giáng sinh
(15, '2025-11-20', '2025-11-21'),  -- Giảm 15% dịp Ngày Nhà giáo Việt Nam
(30, '2025-07-01', '2025-07-10'),  -- Giảm 30% dịp du lịch hè
(12, '2025-04-30', '2025-05-02'),  -- Giảm 12% dịp lễ 30/4 - 1/5
(18, '2025-02-14', '2025-02-15'),  -- Giảm 18% dịp Valentine
(22, '2025-10-20', '2025-10-21'),  -- Giảm 22% dịp Ngày Phụ nữ Việt Nam
(35, '2025-08-15', '2025-08-20'),  -- Giảm 35% dịp mùa du lịch tháng 8
(28, '2025-01-01', '2025-01-05');  -- Giảm 28% dịp Tết Dương lịch
go
CREATE TRIGGER trg_CheckBookingExpiration
ON Bookings
AFTER INSERT, UPDATE
AS
BEGIN
    SET NOCOUNT ON;
    UPDATE b
    SET PaymentStatus = 'Cancelled'
    FROM Bookings b
    INNER JOIN inserted i ON b.BookingID = i.BookingID
    WHERE b.PaymentStatus = 'Pending'
      AND b.ExpirationTime <= GETDATE();
END;
go
select * from users
select * from Hotels
select * from rooms
select * from roomimages
select * from roomtypes
select * from Wishlist
select * from bookings
select * from Discounts
select * from DiscountDetails
go
