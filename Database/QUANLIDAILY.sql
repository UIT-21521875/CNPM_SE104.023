CREATE DATABASE QUANLIDAILY

CREATE TABLE DAILY
(
     MaDaiLy char(5) not null,
	 TenDaiLy nvarchar(25) not null unique,
	 MaLoaiDaiLy char(5) not null,
	 DienThoai char(10),
	 DiaChi nvarchar(50),
	 MaQuan char(5) not null,
	 NgayTiepNhan smalldatetime not null,
	 Email varchar(25),
	 TienNo money default 0
)

CREATE TABLE LOAIDAILY
(
    MaLoaiDaiLy char(5) not null,
	TenLoaiDaiLy nvarchar(25) not null unique,
	SoNoToiDa money default 0
)

CREATE TABLE QUAN
(
     MaQuan char(5) not null,
	 TenQuan nvarchar(15) not null unique
)

CREATE TABLE PHIEUNHAPHANG
(
    MaPhieuNhap char(5) not null,
	NgayNhap smalldatetime not null,
	TongTien money default 0
)

CREATE TABLE CT_PNH
(
	MaPhieuNhap char(5) not null,
	MaMH char(5) not null,
	SoLuongNhap int not null check (SoLuongNhap > 0),
	DonGiaNhap money default 0,
	ThanhTien money default 0
)

CREATE TABLE MATHANG
(
    MaMH char(5) not null,
	TenMH nvarchar(20) not null unique,
	MaDVT char(5) not null,
	SoLuongTon int default 0,
	DonGiaNhap money default 0,
	DonGiaXuat money default 0
)

CREATE TABLE DVT
(
    MaDVT char(5) not null,
	TenDVT nvarchar(20) not null unique
)

CREATE TABLE PHIEUXUATHANG
(
    MaPhieuXuat char(5) not null,
	MaDaiLy char(5) not null,
	NgayXuat smalldatetime not null,
	TongTien money default 0,
	SoTienTra money default 0,
	ConLai money default 0
)

CREATE TABLE CT_PXH
(
	MaPhieuXuat char(5) not null,
	MaMH char(5) not null,
	SoLuongXuat int not null check (SoLuongXuat > 0),
	ThanhTien money default 0,
	DonGiaXuat money default 0
)

CREATE TABLE PHIEUTHUTIEN
(
    MaPhieuThuTien char(5) not null,
	MaDaiLy char(5) not null,
	NgayThuTien smalldatetime not null,
	SoTienThu money not null check(SoTienThu > 0)
)

CREATE TABLE BAOCAODOANHSO
(
    MaBCDS char(6) not null,
	Thang int not null,
	Nam int not null,
	TongDoanhThu money default 0,
)

CREATE TABLE CT_BCDS
(
	MaBCDS char(6) not null,
	MaDaiLy char(5) not null,
	SoPhieuXuat int default 0,
	TongTriGia money default 0,
	TyLe decimal(5,2) default 0,
)

CREATE TABLE BAOCAOCONGNO
(
    MaBCCN char(6) not null,
	Thang int not null,
	Nam int not null,
	TongNo money default 0
)

CREATE TABLE CT_BCCN
(
	MaBCCN char(6) not null,
	MaDaiLy char(5) not null,
	NoDau money default 0,
	PhatSinh money default 0,
	NoCuoi money default 0,
)

CREATE TABLE THAMSO
(
     SoDaiLyToiDa int default 4,
	 TiLeDonGia decimal(5,2) default 102
)

--BỔ SUNG

ALTER TABLE PHIEUXUATHANG
ADD id int IDENTITY(1,1);

ALTER TABLE PHIEUNHAPHANG
ADD id int IDENTITY(1,1);

ALTER TABLE MATHANG
ADD CONSTRAINT So_Luong_Ton_Non_Negative CHECK (SoLuongTon >= 0);





