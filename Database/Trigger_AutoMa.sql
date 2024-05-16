--============================================================ TRIGGER CHO MÃ TỰ ĐỘNG - ALL ==============================================================--

--==========PHIEUNHAPHANG==========--

-- Trình tự cho MaPhieuNhap
CREATE SEQUENCE PHIEUNHAPHANG_MaPhieuNhap_Sequence
    START WITH 1
    INCREMENT BY 1
    MINVALUE 1
    NO MAXVALUE
    CACHE 10;

-- Trigger cho bảng PHIEUNHAPHANG
CREATE TRIGGER PHIEUNHAPHANG_InsertTrigger
ON PHIEUNHAPHANG
INSTEAD OF INSERT
AS
BEGIN
    SET NOCOUNT ON;

    DECLARE @NextMaPhieuNhap VARCHAR(5);
    SET @NextMaPhieuNhap = 'PN' + RIGHT('000' + CAST(NEXT VALUE FOR PHIEUNHAPHANG_MaPhieuNhap_Sequence AS VARCHAR(3)), 3);

    INSERT INTO PHIEUNHAPHANG (MaPhieuNhap, NgayNhap, TongTien)
    SELECT @NextMaPhieuNhap, NgayNhap, TongTien
    FROM inserted;
END;

--========== MATHANG ==========-- 

-- Trình tự cho MaMH
CREATE SEQUENCE MATHANG_MaMH_Sequence
    START WITH 1
    INCREMENT BY 1
    MINVALUE 1
    NO MAXVALUE
    CACHE 10;

-- Trigger cho bảng MATHANG
CREATE TRIGGER MATHANG_InsertTrigger
ON MATHANG
INSTEAD OF INSERT
AS
BEGIN
    SET NOCOUNT ON;

    DECLARE @NextMaMH VARCHAR(5);
    SET @NextMaMH = 'MH' + RIGHT('000' + CAST(NEXT VALUE FOR MATHANG_MaMH_Sequence AS VARCHAR(3)), 3);

    INSERT INTO MATHANG (MaMH, TenMH, MaDVT, SoLuongTon)
    SELECT @NextMaMH, TenMH, MaDVT, SoLuongTon
    FROM inserted;
END;

--========== DVT ==========-- 

-- Trình tự cho MaDVT
CREATE SEQUENCE DVT_MaDVT_Sequence
    START WITH 1
    INCREMENT BY 1
    MINVALUE 1
    NO MAXVALUE
    CACHE 10;

-- Trigger cho bảng DVT
CREATE TRIGGER DVT_InsertTrigger
ON DVT
INSTEAD OF INSERT
AS
BEGIN
    SET NOCOUNT ON;

    DECLARE @NextMaDVT VARCHAR(5);
    SET @NextMaDVT = 'DVT' + RIGHT('00' + CAST(NEXT VALUE FOR DVT_MaDVT_Sequence AS VARCHAR(2)), 2);

    INSERT INTO DVT (MaDVT, TenDVT)
    SELECT @NextMaDVT, TenDVT
    FROM inserted;
END;

--========== DAILY ==========-- 

CREATE SEQUENCE DAILY_MaDaiLy_Sequence
    START WITH 1
    INCREMENT BY 1
    MINVALUE 1
    NO MAXVALUE
    CACHE 10;
    
CREATE TRIGGER DAILY_InsertTrigger
ON DAILY
INSTEAD OF INSERT
AS
BEGIN
    SET NOCOUNT ON;

    DECLARE @NextMaDaiLy VARCHAR(5);
    SET @NextMaDaiLy = 'DL' + RIGHT('000' + CAST(NEXT VALUE FOR DAILY_MaDaiLy_Sequence AS VARCHAR(3)), 3);

    INSERT INTO DAILY (MaDaiLy, TenDaiLy, MaLoaiDaiLy, DienThoai, DiaChi, MaQuan, NgayTiepNhan, Email, TienNo)
    SELECT @NextMaDaiLy, TenDaiLy, MaLoaiDaiLy, DienThoai, DiaChi, MaQuan, NgayTiepNhan, Email, TienNo
    FROM inserted;
END;

--========== PHIEUXUATHANG ==========--

CREATE SEQUENCE PHIEUXUATHANG_MaPhieuXuat_Sequence
    START WITH 1
    INCREMENT BY 1
    MINVALUE 1
    NO MAXVALUE
    CACHE 10;

CREATE TRIGGER PHIEUXUATHANG_InsertTrigger
ON PHIEUXUATHANG
INSTEAD OF INSERT
AS
BEGIN
    SET NOCOUNT ON;

    DECLARE @NextMaPhieuXuat VARCHAR(5);
    SET @NextMaPhieuXuat = 'PX' + RIGHT('000' + CAST(NEXT VALUE FOR PHIEUXUATHANG_MaPhieuXuat_Sequence AS VARCHAR(3)), 3);

    INSERT INTO PHIEUXUATHANG (MaPhieuXuat, MaDaiLy, NgayXuat, TongTien, SoTienTra, ConLai)
    SELECT @NextMaPhieuXuat, MaDaiLy, NgayXuat, TongTien, SoTienTra, ConLai
    FROM inserted;
END;


--========== QUAN ==========--

CREATE SEQUENCE QUAN_MaQuan_Sequence
    START WITH 1
    INCREMENT BY 1
    MINVALUE 1
    NO MAXVALUE
    CACHE 10;
    
CREATE TRIGGER QUAN_InsertTrigger
ON QUAN
INSTEAD OF INSERT
AS
BEGIN
    SET NOCOUNT ON;

    DECLARE @NextMaQuan VARCHAR(5);
    SET @NextMaQuan = 'Q' + RIGHT('0000' + CAST(NEXT VALUE FOR QUAN_MaQuan_Sequence AS VARCHAR(4)), 4);

    INSERT INTO QUAN (MaQuan, TenQuan)
    SELECT @NextMaQuan, TenQuan
    FROM inserted;
END;

--========== LOAIDAILY ==========--

CREATE SEQUENCE LOAIDAILY_MaLoaiDaiLy_Sequence
    START WITH 1
    INCREMENT BY 1
    MINVALUE 1
    NO MAXVALUE
    CACHE 10;

CREATE TRIGGER LOAIDAILY_InsertTrigger
ON LOAIDAILY
INSTEAD OF INSERT
AS
BEGIN
    SET NOCOUNT ON;

    DECLARE @NextMaLoaiDaiLy VARCHAR(5);
    SET @NextMaLoaiDaiLy = 'LDL' + RIGHT('00' + CAST(NEXT VALUE FOR LOAIDAILY_MaLoaiDaiLy_Sequence AS VARCHAR(2)), 2);

    INSERT INTO LOAIDAILY (MaLoaiDaiLy, TenLoaiDaiLy, SoNoToiDa)
    SELECT @NextMaLoaiDaiLy, TenLoaiDaiLy, SoNoToiDa
    FROM inserted;
END;

--========== PHIEUTHUTIEN ==========--

CREATE SEQUENCE PHIEUTHUTIEN_MaPhieuThuTien_Sequence
    START WITH 1
    INCREMENT BY 1
    MINVALUE 1
    NO MAXVALUE
    CACHE 10;

CREATE TRIGGER PHIEUTHUTIEN_InsertTrigger
ON PHIEUTHUTIEN
INSTEAD OF INSERT
AS
BEGIN
    SET NOCOUNT ON;

    DECLARE @NextMaPhieuThuTien VARCHAR(5);
    SET @NextMaPhieuThuTien = 'PT' + RIGHT('000' + CAST(NEXT VALUE FOR PHIEUTHUTIEN_MaPhieuThuTien_Sequence AS VARCHAR(3)), 3);

    INSERT INTO PHIEUTHUTIEN (MaPhieuThuTien, MaDaiLy, NgayThuTien, SoTienThu)
    SELECT @NextMaPhieuThuTien, MaDaiLy, NgayThuTien, SoTienThu
    FROM inserted;
END;

