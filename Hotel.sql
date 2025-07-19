--CREATE DATABASE Hotel
GO
USE Hotel
GO
-- 1. Các bảng phụ có FOREIGN KEY đến bảng khác
DROP TABLE IF EXISTS RoomViews;
DROP TABLE IF EXISTS RoomImages;
DROP TABLE IF EXISTS RoomAmenities;
DROP TABLE IF EXISTS Wishlist;
DROP TABLE IF EXISTS DiscountDetails;
DROP TABLE IF EXISTS Reviews;
DROP TABLE IF EXISTS Bookings;
-- 2. Các bảng trung gian/phụ thuộc nhưng không phụ thuộc lẫn nhau
DROP TABLE IF EXISTS Discounts;
DROP TABLE IF EXISTS Rooms;
-- 3. Các bảng gốc không phụ thuộc bảng khác (hoặc được tham chiếu)
DROP TABLE IF EXISTS RoomTypes;
DROP TABLE IF EXISTS Amenities;
DROP TABLE IF EXISTS Views;
DROP TABLE IF EXISTS Hotels;
DROP TABLE IF EXISTS Provinces;
DROP TABLE IF EXISTS Regions;
DROP TABLE IF EXISTS Users;

GO
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
VALUES ('admin', 'admin@gmail.com', 'admin', '$2a$11$mbavRwTjp9o9oHk1NJBvwugHzRiuUKJCCF2UXNNKmJDcmUi5OXbcq', 'Admin');
GO

CREATE TABLE Regions (
    RegionID INT PRIMARY KEY IDENTITY(1,1),
    RegionName NVARCHAR(50) NOT NULL
);
GO
-- Bảng tỉnh/thành
CREATE TABLE Provinces (
    ProvinceID INT PRIMARY KEY,
    ProvinceName NVARCHAR(100) NOT NULL,
    RegionID INT NOT NULL,
    FOREIGN KEY (RegionID) REFERENCES Regions(RegionID)
);
CREATE TABLE Hotels (
    HotelID INT PRIMARY KEY IDENTITY(1,1),
    HotelName NVARCHAR(255) NOT NULL,
    Address NVARCHAR(255) NOT NULL,
	HotelImage NVARCHAR(255) ,
	ProvinceID INT NOT NULL,
    Latitude DECIMAL(9,6) NOT NULL, 
    Longitude DECIMAL(9,6) NOT NULL, 
	FOREIGN KEY (ProvinceID) REFERENCES Provinces(ProvinceID),
);
GO
CREATE TABLE RoomTypes (
    RoomTypeId INT PRIMARY KEY IDENTITY(1,1),
    RoomTypeName NVARCHAR(100) NOT NULL
);
GO
CREATE TABLE Amenities (
    AmenityID INT PRIMARY KEY IDENTITY(1,1),
    AmenityName NVARCHAR(100) NOT NULL,
	AmenityIcon NVARCHAR(100),
);
GO
CREATE TABLE Views (
    ViewID INT PRIMARY KEY IDENTITY(1,1),
    ViewName NVARCHAR(100) NOT NULL,
	ViewIcon NVARCHAR (100),
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
	Thumnail NVARCHAR(MAX),
	AllowSmoking BIT DEFAULT 0,
    AllowPet BIT DEFAULT 0,
	CancellationPolicy NVARCHAR(100),
    Description NVARCHAR(MAX),
    FOREIGN KEY (HotelID) REFERENCES Hotels(HotelID) ON DELETE CASCADE,
    FOREIGN KEY (RoomTypeId) REFERENCES RoomTypes(RoomTypeId) ON DELETE CASCADE
);
GO
-- Tạo bảng ảnh phòng
CREATE TABLE RoomImages (
    ImageID INT PRIMARY KEY IDENTITY(1,1),
    RoomID INT NOT NULL,
    ImageURL NVARCHAR(MAX) NOT NULL,
    FOREIGN KEY (RoomID) REFERENCES Rooms(RoomID) ON DELETE CASCADE
);
GO
CREATE TABLE RoomAmenities (
    RoomID INT NOT NULL,
    AmenityID INT NOT NULL,
    PRIMARY KEY (RoomID, AmenityID),
    FOREIGN KEY (RoomID) REFERENCES Rooms(RoomID) ON DELETE CASCADE,
    FOREIGN KEY (AmenityID) REFERENCES Amenities(AmenityID) ON DELETE CASCADE
);
GO
CREATE TABLE RoomViews (
    RoomID INT NOT NULL,
    ViewID INT NOT NULL,
    PRIMARY KEY (RoomID, ViewID),
    FOREIGN KEY (RoomID) REFERENCES Rooms(RoomID) ON DELETE CASCADE,
    FOREIGN KEY (ViewID) REFERENCES Views(ViewID) ON DELETE CASCADE
);
GO
CREATE TABLE Discounts (
    DiscountID INT PRIMARY KEY IDENTITY(1,1),
    DiscountPercent INT CHECK (DiscountPercent BETWEEN 0 AND 100) NOT NULL,
    StartDate DATE NOT NULL,
    EndDate DATE NOT NULL,
	);
GO
CREATE TABLE DiscountDetails (
    DiscountDetailID INT PRIMARY KEY IDENTITY(1,1),
    DiscountID INT NOT NULL,
    RoomID INT NOT NULL UNIQUE,
    FOREIGN KEY (DiscountID) REFERENCES Discounts(DiscountID) ON DELETE CASCADE,
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
