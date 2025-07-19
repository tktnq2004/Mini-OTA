USE Hotel
GO
INSERT INTO Regions (RegionName)
VALUES 
(N'Mi?n B?c'),
(N'Mi?n Trung'),
(N'Mi?n Nam');
GO
INSERT INTO Provinces (ProvinceID, ProvinceName, RegionID) VALUES
-- Mi?n B?c (RegionID = 1)
(11, N'Cao Bằng', 1),
(12, N'Lạng Sơn', 1),
(14, N'Quảng Ninh', 1),
(15, N'Hải Phòng', 1),
(17, N'Thái Bình', 1),
(18, N'Nam Định', 1),
(19, N'Phú Thọ', 1),
(20, N'Thái Nguyên', 1),
(21, N'Yên Bái', 1),
(22, N'Tuyên Quang', 1),
(23, N'Hà Giang', 1),
(24, N'Lào Cai', 1),
(25, N'Lai Châu', 1),
(26, N'Sơn La', 1),
(27, N'Điện Biên', 1),
(28, N'Hòa Bình', 1),
(29, N'Hà Nội', 1),
(34, N'Hải Dương', 1),
(35, N'Ninh Bình', 1),
(36, N'Thanh Hóa', 1),
(88, N'Vĩnh Phúc', 1),
(89, N'Hưng Yên', 1),
(90, N'Hà Nam', 1),
(97, N'Bắc Kạn', 1),
(98, N'Bắc Giang', 1),
(99, N'Bắc Ninh', 1),

-- Mi?n Trung (RegionID = 2)
(37, N'Nghệ An', 2),
(38, N'Hà Tỉnh', 2),
(43, N'Đà Nẵng', 2),
(47, N'Đăk Lăk', 2),
(48, N'Đăk Nông', 2),
(49, N'Lâm Đồng', 2),
(73, N'Quảng Bình', 2),
(74, N'Quảng Trị', 2),
(75, N'Thừa Thiên Huế', 2),
(76, N'Quảng Ngãi', 2),
(77, N'Bình Định', 2),
(78, N'Phú Yên', 2),
(79, N'Khánh Hòa', 2),
(85, N'Ninh Thuận', 2),
(86, N'Bình Thuận', 2),
(92, N'Quảng Nam', 2),
(81, N'Gia Lai', 2),
(82, N'Kon Tum', 2),

-- Mi?n Nam (RegionID = 3)
(50, N'TP. Hồ Chí Minh', 3),
(60, N'Đồng Nai', 3),
(61, N'Bình Dương', 3),
(62, N'Long An', 3),
(63, N'Tiền Giang', 3),
(64, N'Vịnh Long', 3),
(65, N'Cần Thơ', 3),
(66, N'Đồng Tháp', 3),
(67, N'An Giang', 3),
(68, N'Kiên Giang', 3),
(69, N'Cà Mau', 3),
(70, N'Tây Ninh', 3),
(71, N'Bến Tre', 3),
(72, N'Bà Rịa - Vũng Tàu', 3),
(83, N'Sóc Trăng', 3),
(84, N'Trà Vinh', 3),
(93, N'Bình Phước', 3),
(94, N'Bạc Liêu', 3),
(95, N'Hậu Giang', 3);

GO
INSERT INTO RoomTypes (RoomTypeName) VALUES
('Single'),
('Double'),
('Twin'),
('Standard'),
('Deluxe'),
('Suite');
GO
-- Chèn d? li?u vào b?ng RoomTypes
INSERT INTO RoomTypes (RoomTypeName) VALUES
('Standard Room'),
('Deluxe Room'),
('Suite'),
('Family Room'),
('Executive Room');
GO
INSERT INTO Amenities (AmenityName, AmenityIcon) VALUES
(N'Air conditioning',        'bi-snow'),
(N'Free Wi-Fi',              'bi-wifi'),
(N'Bathtub',                 'bi-droplet'),
(N'In-room slippers',        'bi-slack'),       
(N'Room service',            'bi-bell'),
(N'Balcony',                 'bi-house-door'),
(N'Breakfast included',      'bi-cup-hot');
GO
INSERT INTO Views (ViewName, ViewIcon) VALUES
(N'Sea view',           'bi-water'),
(N'Lake view',          'bi-droplet-half'),
(N'Garden view',        'bi-flower1'),
(N'Mountain view',      'bi-triangle'),
(N'River view',         'bi-signpost'),
(N'Street view',        'bi-signpost-2'),
(N'City center view',   'bi-building');
GO
-- Chèn d? li?u vào b?ng Hotels
INSERT INTO Hotels (HotelName, Address, HotelImage, ProvinceID, Latitude, Longitude) VALUES
('Sofitel Legend Metropole Hanoi', '15 Ngo Quyen, Hoan Kiem, Hanoi', '/Content/hotelimages/hotel1.jpg', 29, 21.0251, 105.8557),
('Hotel Nikko Saigon', '235 Nguyen Van Cu, District 1, Ho Chi Minh City', '/Content/hotelimages/hotel2.jpg', 50, 10.7710, 106.6930),
('Hyatt Regency Danang Resort and Spa', '5 Truong Sa, Ngu Hanh Son, Da Nang', '/Content/hotelimages/hotel3.jpg', 43, 16.0167, 108.2333),
('The Reverie Saigon', '22-36 Nguyen Hue, District 1, Ho Chi Minh City', '/Content/hotelimages/hotel4.jpg', 50, 10.7744, 106.7037),
('Furama Resort Danang', '103-105 Vo Nguyen Giap, Ngu Hanh Son, Da Nang', '/Content/hotelimages/hotel5.jpg', 43, 16.0400, 108.2500),
('InterContinental Hanoi Westlake', '5 Tu Hoa, Tay Ho, Hanoi', '/Content/hotelimages/hotel6.jpg', 29, 21.0587, 105.8290),
('Mai House Saigon Hotel', '157 Nam Ky Khoi Nghia, District 3, Ho Chi Minh City', '/Content/hotelimages/hotel7.jpg', 50, 10.7867, 106.6890),
('Four Seasons Resort The Nam Hai', 'Block Ha My Dong B, Dien Ban, Hoi An', '/Content/hotelimages/hotel8.jpg', 92, 15.9167, 108.3167),
('Azerai Can Tho', 'Con Au, Cai Rang, Can Tho', '/Content/hotelimages/hotel9.jpg', 65, 10.0167, 105.7833),
('Capella Hanoi', '11 Le Phung Hieu, Hoan Kiem, Hanoi', '/Content/hotelimages/hotel10.jpg', 29, 21.0240, 105.8560),
('Vinpearl Resort & Spa Phu Quoc', 'Bai Dai, Ganh Dau, Phu Quoc', '/Content/hotelimages/hotel11.jpg', 68, 10.3333, 103.9167),
('Hotel des Arts Saigon', '76-78 Nguyen Thi Minh Khai, District 3, Ho Chi Minh City', '/Content/hotelimages/hotel12.jpg', 50, 10.7860, 106.6940),
('Six Senses Con Dao', 'Dat Doc Beach, Con Dao', '/Content/hotelimages/hotel13.jpg', 68, 8.6833, 106.6167),
('Ana Mandara Villas Dalat', 'Le Lai, Ward 5, Dalat', '/Content/hotelimages/hotel14.jpg', 49, 11.9333, 108.4500),
('The Anam Cam Ranh', 'Nguyen Tat Thanh, Cam Ranh, Khanh Hoa', '/Content/hotelimages/hotel15.jpg', 79, 12.0000, 109.2000),
('Poulo Condor Boutique Resort & Spa', 'Suoi Lon, Con Son Island', '/Content/hotelimages/hotel16.jpg', 72, 8.6833, 106.6167),
('Avana Retreat', 'Mai Chau, Hoa Binh', '/Content/hotelimages/hotel17.jpg', 28, 20.6667, 105.0667),
('Bach Suites Saigon', '10A Pham Ngoc Thach, District 3, Ho Chi Minh City', '/Content/hotelimages/hotel18.jpg', 50, 10.7850, 106.6950),
('Le Meridien Saigon', '3C Ton Duc Thang, District 1, Ho Chi Minh City', '/Content/hotelimages/hotel19.jpg', 50, 10.7810, 106.7060),
('Hotel Royal Hoi An', '39 Dao Duy Tu, Hoi An', '/Content/hotelimages/hotel20.jpg', 92, 15.8800, 108.3300),
('Mercure Danang French Village Bana Hills', 'An Son, Hoa Ninh, Da Nang', '/Content/hotelimages/hotel21.jpg', 43, 16.0000, 108.0000),
('Peridot Grand Luxury Boutique Hotel', '33 Duong Le Loi, Hue', '/Content/hotelimages/hotel22.jpg', 75, 16.4667, 107.5833),
('Lotte Hotel Hanoi', '54 Lieu Giai, Ba Dinh, Hanoi', '/Content/hotelimages/hotel23.jpg', 29, 21.0333, 105.8167),
('Melia Vinpearl Nha Trang Empire', '44-46 Le Thanh Ton, Nha Trang', '/Content/hotelimages/hotel24.jpg', 79, 12.2500, 109.2000),
('New World Phu Quoc Resort', 'Kem Beach, An Thoi, Phu Quoc', '/Content/hotelimages/hotel25.jpg', 68, 10.0333, 104.0167),

--
(N'Anantara Hoi An Resort', N'1 Pham Hong Thai, Cam Chau, Hoi An', '/Content/hotelimages/hotel1.jpg', 92, 15.8870, 108.3260),
(N'JW Marriott Phu Quoc Emerald Bay Resort & Spa', N'Bai Khem, An Thoi, Phu Quoc', '/Content/hotelimages/hotel2.jpg', 68, 10.0330, 104.0160),
(N'Six Senses Ninh Van Bay', N'Ninh Van Bay, Ninh Hoa, Khanh Hoa', '/Content/hotelimages/hotel3.jpg', 79, 12.3667, 109.2333),
(N'Fusion Maia Da Nang', N'Vo Nguyen Giap, Ngu Hanh Son, Da Nang', '/Content/hotelimages/hotel4.jpg', 43, 16.0500, 108.2500),
(N'La Siesta Premium Saigon', N'33-35 Le Thanh Ton, District 1, Ho Chi Minh City', '/Content/hotelimages/hotel5.jpg', 50, 10.7800, 106.7050),
(N'Amanoi Resort', N'Vinh Hy Village, Vinh Hai, Ninh Thuan', '/Content/hotelimages/hotel6.jpg', 85, 11.7167, 109.1833),
(N'La Siesta Classic Hang Thung', N'94 Hang Thung, Hoan Kiem, Hanoi', '/Content/hotelimages/hotel7.jpg', 29, 21.0310, 105.8530),
(N'Melia Danang Beach Resort', N'19 Truong Sa, Ngu Hanh Son, Da Nang', '/Content/hotelimages/hotel8.jpg', 43, 16.0300, 108.2400),
(N'Vinpearl Resort & Golf Nam Hoi An', N'Binh Duong, Binh Minh, Hoi An', '/Content/hotelimages/hotel9.jpg', 92, 15.8500, 108.3000),
(N'Grand Hotel du Lac Hanoi', N'18-20-22-24 Nguyen Hue, Hoan Kiem, Hanoi', '/Content/hotelimages/hotel10.jpg', 29, 21.0280, 105.8540),
(N'Son Hoi An Boutique Hotel & Spa', N'48 Nguyen Thi Minh Khai, Hoi An', '/Content/hotelimages/hotel11.jpg', 92, 15.8800, 108.3300),
(N'Muong Thanh Luxury Da Nang Hotel', N'270 Vo Nguyen Giap, Ngu Hanh Son, Da Nang', '/Content/hotelimages/hotel12.jpg', 43, 16.0450, 108.2450),
(N'Liberty Central Saigon Citypoint', N'59-61 Pasteur, District 1, Ho Chi Minh City', '/Content/hotelimages/hotel13.jpg', 50, 10.7790, 106.7010),
(N'Pao’s Sapa Leisure Hotel', N'Muong Hoa, Sapa, Lao Cai', '/Content/hotelimages/hotel14.jpg', 24, 22.3360, 103.8430),
(N'Little Riverside Hoi An', N'09 Phan Boi Chau, Hoi An', '/Content/hotelimages/hotel15.jpg', 92, 15.8850, 108.3250),
(N'M Hotel Danang', N'81 Vo Van Kiet, Ngu Hanh Son, Da Nang', '/Content/hotelimages/hotel16.jpg', 43, 16.0600, 108.2300),
(N'Wyndham Hoi An Royal Beachfront Resort & Villas', N'Ha My Beach, Dien Duong, Hoi An', '/Content/hotelimages/hotel17.jpg', 92, 15.9100, 108.3200),
(N'TTC Imperial Hotel', N'159 Hung Vuong, Hue', '/Content/hotelimages/hotel18.jpg', 75, 16.4650, 107.5900),
(N'Vinpearl Beachfront Nha Trang', N'78-80 Tran Phu, Nha Trang', '/Content/hotelimages/hotel19.jpg', 79, 12.2450, 109.1950),
(N'Melia Ho Tram Beach Resort', N'Ho Tram, Xuyen Moc, Ba Ria - Vung Tau', '/Content/hotelimages/hotel20.jpg', 72, 10.4667, 107.4500),
(N'Sedona Suites Ho Chi Minh City', N'67 Le Loi, District 1, Ho Chi Minh City', '/Content/hotelimages/hotel21.jpg', 50, 10.7740, 106.7020),
(N'Fusion Original Saigon Centre', N'65 Le Loi, District 1, Ho Chi Minh City', '/Content/hotelimages/hotel22.jpg', 50, 10.7730, 106.7010),
(N'Hotel de la Coupole - MGallery', N'1 Hoang Lien, Sapa, Lao Cai', '/Content/hotelimages/hotel23.jpg', 94, 22.3350, 103.8400),
(N'Da Nang Mikazuki Japanese Resort & Spa', N'Xuan Thieu, Hoa Hiep Nam, Da Nang', '/Content/hotelimages/hotel24.jpg', 43, 16.1000, 108.2000),
(N'Mercure Dalat Resort', N'3 Nguyen Du, Dalat', '/Content/hotelimages/hotel25.jpg', 49, 11.9400, 108.4400),

-- Chèn 20 khách s?n ? TP. H? Chí Minh
(N'Silverland Yen Hotel', N'111-113 Ly Tu Trong, District 1, Ho Chi Minh City', '/Content/hotelimages/hotel1.jpg', 50, 10.7730, 106.6990),
(N'Liberty Central Saigon Riverside', N'17 Ton Duc Thang, District 1, Ho Chi Minh City', '/Content/hotelimages/hotel2.jpg', 50, 10.7800, 106.7070),
(N'New World Saigon Hotel', N'76 Le Lai Street, District 1, Ho Chi Minh City', '/Content/hotelimages/hotel3.jpg', 50, 10.7710, 106.6970),
(N'Park Hyatt Saigon', N'2 Lam Son Square, District 1, Ho Chi Minh City', '/Content/hotelimages/hotel4.jpg', 50, 10.7770, 106.7030),
(N'Icon Saigon Luxury Hotel', N'65-67 Hai Ba Trung, District 1, Ho Chi Minh City', '/Content/hotelimages/hotel5.jpg', 50, 10.7800, 106.7020),
(N'Sherwood Suites', N'192-194 Nam Ky Khoi Nghia, District 3, Ho Chi Minh City', '/Content/hotelimages/hotel6.jpg', 50, 10.7860, 106.6880),
(N'Sofitel Saigon Plaza', N'17 Le Duan Boulevard, District 1, Ho Chi Minh City', '/Content/hotelimages/hotel7.jpg', 50, 10.7840, 106.7020),
(N'Grand Hotel Saigon', N'8 Dong Khoi Street, District 1, Ho Chi Minh City', '/Content/hotelimages/hotel8.jpg', 50, 10.7750, 106.7040),
(N'Caravelle Saigon', N'19-23 Lam Son Square, District 1, Ho Chi Minh City', '/Content/hotelimages/hotel9.jpg', 50, 10.7760, 106.7030),
(N'An Lam Retreats Saigon River', N'21/4 Trung Village, Vinh Phu, Thuan An, Ho Chi Minh City', '/Content/hotelimages/hotel10.jpg', 50, 10.8500, 106.7160),
(N'Alagon D’antique Hotel & Spa', N'301-303 Ly Tu Trong, District 1, Ho Chi Minh City', '/Content/hotelimages/hotel11.jpg', 50, 10.7720, 106.6980),
(N'Saigon Prince Hotel', N'63 Nguyen Hue, District 1, Ho Chi Minh City', '/Content/hotelimages/hotel12.jpg', 50, 10.7740, 106.7040),
(N'Harmony Saigon Hotel & Spa', N'32A-34 Bui Thi Xuan, District 1, Ho Chi Minh City', '/Content/hotelimages/hotel13.jpg', 50, 10.7700, 106.6900),
(N'Eden Star Saigon Hotel', N'38 Bui Thi Xuan, District 1, Ho Chi Minh City', '/Content/hotelimages/hotel14.jpg', 50, 10.7700, 106.6910),
(N'Mai House Saigon Hotel', N'1 Ngo Thoi Nhiem, District 3, Ho Chi Minh City', '/Content/hotelimages/hotel15.jpg', 50, 10.7798, 106.6860),
(N'La Vela Saigon Hotel', N'280 Nam Ky Khoi Nghia, District 3, Ho Chi Minh City', '/Content/hotelimages/hotel16.jpg', 50, 10.7835, 106.6843),
(N'Silverland Jolie Hotel & Spa', N'4D Thi Sach, District 1, Ho Chi Minh City', '/Content/hotelimages/hotel17.jpg', 50, 10.7790, 106.7040),
(N'Villa Song Saigon', N'197/2 Nguyen Van Huong, District 2, Ho Chi Minh City', '/Content/hotelimages/hotel18.jpg', 50, 10.8020, 106.7360),
(N'The Myst Dong Khoi', N'6-8 Ho Huan Nghiep, District 1, Ho Chi Minh City', '/Content/hotelimages/hotel19.jpg', 50, 10.7760, 106.7060),
(N'Pullman Saigon Centre', N'148 Tran Hung Dao, District 1, Ho Chi Minh City', '/Content/hotelimages/hotel20.jpg', 50, 10.7673, 106.6921),
(N'Vinpearl Landmark 81, Autograph Collection', N'720A Dien Bien Phu, Binh Thanh District, Ho Chi Minh City', '/Content/hotelimages/hotel21.jpg', 50, 10.7960, 106.7210),
(N'Au Lac Legend Hotel', N'90 Nguyen Thi Minh Khai, District 3, Ho Chi Minh City', '/Content/hotelimages/hotel22.jpg', 50, 10.7840, 106.6930),
(N'Hotel Nikko Saigon', N'235 Nguyen Van Cu, District 1, Ho Chi Minh City', '/Content/hotelimages/hotel23.jpg', 50, 10.7625, 106.6812),
(N'Fusion Suites Saigon', N'3-5 Suong Nguyet Anh, District 1, Ho Chi Minh City', '/Content/hotelimages/hotel24.jpg', 50, 10.7690, 106.6900),
(N'District 1 Test Hotel', N'123 Test Street, District 1, Ho Chi Minh City', '/Content/hotelimages/hotel25.jpg', 50, 10.7740, 106.7040),

-- Chèn 20 khách s?n ? Hà N?i
(N'Apricot Hotel', N'136 Hang Trong, Hoan Kiem, Hanoi', '/Content/hotelimages/hotel17.jpg', 29, 21.0280, 105.8510),
(N'Hotel du Parc Hanoi', N'84 Tran Nhan Tong, Hai Ba Trung, Hanoi', '/Content/hotelimages/hotel4.jpg', 29, 21.0160, 105.8460),
(N'Melia Hanoi', N'44B Ly Thuong Kiet, Hoan Kiem, Hanoi', '/Content/hotelimages/hotel13.jpg', 29, 21.0250, 105.8520),
(N'JW Marriott Hotel Hanoi', N'8 Do Duc, Nam Tu Liem, Hanoi', '/Content/hotelimages/hotel9.jpg', 29, 21.0280, 105.7830),
(N'Pan Pacific Hanoi', N'1 Thanh Nien, Ba Dinh, Hanoi', '/Content/hotelimages/hotel22.jpg', 29, 21.0450, 105.8400),
(N'Fraser Suites Hanoi', N'51 Xuan Dieu, Tay Ho, Hanoi', '/Content/hotelimages/hotel2.jpg', 29, 21.0600, 105.8300),
(N'Hanoi La Siesta Hotel & Spa', N'94 Ma May, Hoan Kiem, Hanoi', '/Content/hotelimages/hotel7.jpg', 29, 21.0340, 105.8540),
(N'The Oriental Jade Hotel', N'92-94 Hang Trong, Hoan Kiem, Hanoi', '/Content/hotelimages/hotel20.jpg', 29, 21.0280, 105.8500),
(N'Sofitel Legend Metropole Hanoi', N'15 Ngo Quyen, Hoan Kiem, Hanoi', '/Content/hotelimages/hotel11.jpg', 29, 21.0251, 105.8557),
(N'Hanoi Brilliant Hotel & Spa', N'44 Hang Trong, Hoan Kiem, Hanoi', '/Content/hotelimages/hotel5.jpg', 29, 21.0280, 105.8510),
(N'Peridot Grand Hotel & Spa', N'33 Duong Thanh, Hoan Kiem, Hanoi', '/Content/hotelimages/hotel19.jpg', 29, 21.0290, 105.8470),
(N'Hanoi Pearl Hotel', N'6 Bao Khanh, Hoan Kiem, Hanoi', '/Content/hotelimages/hotel14.jpg', 29, 21.0310, 105.8520),
(N'The Lapis Hotel', N'21 Tran Hung Dao, Hoan Kiem, Hanoi', '/Content/hotelimages/hotel24.jpg', 29, 21.0220, 105.8520),
(N'Elegant Suites Westlake', N'10B Dang Thai Mai, Tay Ho, Hanoi', '/Content/hotelimages/hotel3.jpg', 29, 21.0580, 105.8280),
(N'Silk Path Grand Hue Hotel', N'2 Le Loi, Hue, Hanoi', '/Content/hotelimages/hotel16.jpg', 29, 21.0260, 105.8490),
(N'Hanoi La Storia Hotel', N'45-47 Hang Dong, Hoan Kiem, Hanoi', '/Content/hotelimages/hotel10.jpg', 29, 21.0320, 105.8490),
(N'Mövenpick Hotel Hanoi', N'83A Ly Thuong Kiet, Hoan Kiem, Hanoi', '/Content/hotelimages/hotel6.jpg', 29, 21.0240, 105.8520),
(N'Hanoi Emerald Waters Hotel & Spa', N'47 Lo Su, Hoan Kiem, Hanoi', '/Content/hotelimages/hotel12.jpg', 29, 21.0330, 105.8540),
(N'The Light Hotel', N'128-130 Hang Bong, Hoan Kiem, Hanoi', '/Content/hotelimages/hotel1.jpg', 29, 21.0290, 105.8480),
(N'Hanoi Elite Hotel', N'10/50 Dao Duy Tu, Hoan Kiem, Hanoi', '/Content/hotelimages/hotel21.jpg', 29, 21.0350, 105.8530),
(N'Lotte Hotel Hanoi', N'54 Lieu Giai, Ba Dinh, Hanoi', '/Content/hotelimages/hotel8.jpg', 29, 21.0358, 105.8148),
(N'Pullman Hanoi', N'40 Cat Linh, Dong Da, Hanoi', '/Content/hotelimages/hotel18.jpg', 29, 21.0325, 105.8307),
(N'Diamond Westlake Suites', N'96 To Ngoc Van, Tay Ho, Hanoi', '/Content/hotelimages/hotel15.jpg', 29, 21.0632, 105.8214),
(N'Hilton Hanoi Opera', N'1 Le Thanh Tong, Hoan Kiem, Hanoi', '/Content/hotelimages/hotel23.jpg', 29, 21.0233, 105.8572),
(N'Cosiana Hotel Hanoi', N'92 Le Duan, Dong Da, Hanoi', '/Content/hotelimages/hotel25.jpg', 29, 21.0295, 105.8394);

GO
EXEC InsertAllRoomFeatures;
GO
DECLARE @HotelID INT = 1;
WHILE @HotelID <= 100
BEGIN
    INSERT INTO Rooms (HotelID, RoomTypeId, RoomName, Price, Capacity, Thumnail, Description) VALUES
    (@HotelID, 1, 'Standard Room ' + CAST(@HotelID AS NVARCHAR), 100.00, 2, '/Content/roomthumbnail/P101/101_1.jpg' + CAST(@HotelID AS NVARCHAR) + '.jpg', 'Cozy standard room with modern amenities'),
    (@HotelID, 2, 'Deluxe Room ' + CAST(@HotelID AS NVARCHAR), 150.00, 2, '/Content/roomthumbnail/P101/101_2.jpg' + CAST(@HotelID AS NVARCHAR) + '.jpg', 'Spacious deluxe room with city view'),
    (@HotelID, 3, 'Suite ' + CAST(@HotelID AS NVARCHAR), 250.00, 4, '/Content/roomthumbnail/P101/101_3.jpg' + CAST(@HotelID AS NVARCHAR) + '.jpg', 'Luxurious suite with premium facilities'),
    (@HotelID, 4, 'Family Room ' + CAST(@HotelID AS NVARCHAR), 200.00, 6, '/Content/roomthumbnail/P101/101_4.jpg' + CAST(@HotelID AS NVARCHAR) + '.jpg', 'Family-friendly room with extra space'),
    (@HotelID, 5, 'Executive Room ' + CAST(@HotelID AS NVARCHAR), 180.00, 3, '/Content/roomthumbnail/P101/101_5.jpg' + CAST(@HotelID AS NVARCHAR) + '.jpg', 'Elegant room for business travelers');
    SET @HotelID = @HotelID + 1;
END;
DECLARE @RoomID INT = 1;

WHILE @RoomID <= 100  -- thay 100 bằng số lượng phòng của bạn
BEGIN
    DECLARE @RandomFolder INT = FLOOR(RAND() * 10) + 1;     
    DECLARE @RandomSuffix INT = FLOOR(RAND() * 5) + 1;           -- 1 → 5

    DECLARE @ImageURL NVARCHAR(255) = '/Content/images/' + 
                                      CAST(@RandomFolder AS NVARCHAR) + 
                                      '/P' + CAST(@RandomFolder AS NVARCHAR)+
									  '0'+
                                      CAST(@RandomSuffix AS NVARCHAR) + 
                                      '.jpg';

    INSERT INTO RoomImages (RoomID, ImageURL)
    VALUES (@RoomID, @ImageURL);

    SET @RoomID = @RoomID + 1;
END;

go
INSERT INTO Discounts (DiscountPercent, StartDate, EndDate) 
VALUES 
(10, '2025-06-01', '2025-06-05'),  -- Gi?m 10% d?p Qu?c t? Thi?u nhi
(25, '2025-09-01', '2025-09-03'),  -- Gi?m 25% d?p Qu?c khánh 2/9
(20, '2025-12-20', '2025-12-31'),  -- Gi?m 20% d?p Giáng sinh
(15, '2025-11-20', '2025-11-21'),  -- Gi?m 15% d?p Ngày Nhà giáo Vi?t Nam
(30, '2025-07-01', '2025-07-10'),  -- Gi?m 30% d?p du l?ch hè
(12, '2025-04-30', '2025-05-02'),  -- Gi?m 12% d?p l? 30/4 - 1/5
(18, '2025-02-14', '2025-02-15'),  -- Gi?m 18% d?p Valentine
(22, '2025-10-20', '2025-10-21'),  -- Gi?m 22% d?p Ngày Ph? n? Vi?t Nam
(35, '2025-08-15', '2025-08-20'),  -- Gi?m 35% d?p mùa du l?ch tháng 8
(28, '2025-01-01', '2025-01-05');  -- Gi?m 28% d?p T?t D??ng l?ch
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
select * from Provinces
select * from Views
select * from Amenities
go
