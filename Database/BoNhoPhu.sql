CREATE TABLE PHIEUXUATHANG_PHU
(
    id int IDENTITY(1,1) PRIMARY KEY,
    NgayXuat smalldatetime,
    TongTien money default 0,
    SoTienTra money default 0,
    ConLai AS (TongTien - SoTienTra)
)

CREATE TABLE CT_PXH_PHU
(
    id int,
	MaMH char(5),
	SoLuongXuat int default 1 check (SoLuongXuat > 0),
	DonGiaXuat money default 0,
	ThanhTien money default 0,
	CONSTRAINT PK_phu PRIMARY KEY (id, MaMH)
)

--=============================== BỘ NHỚ PHỤ XUẤT HÀNG ==========================================
CREATE TRIGGER UpdateTongTien
ON CT_PXH_PHU
AFTER INSERT, UPDATE, DELETE
AS
BEGIN
    -- Cập nhật ThanhTien cho các CT_PXH_PHU
    UPDATE CT
    SET CT.ThanhTien = CT.SoLuongXuat * CT.DonGiaXuat
    FROM CT_PXH_PHU CT
    INNER JOIN inserted I ON CT.id = I.id AND CT.MaMH = I.MaMH
    WHERE I.id IS NOT NULL;

    -- Cập nhật TongTien trong PHIEUXUATHANG_PHU
    UPDATE PH
    SET PH.TongTien = (
        SELECT SUM(CT.ThanhTien)
        FROM CT_PXH_PHU CT
        WHERE PH.id = CT.id
    )
    FROM PHIEUXUATHANG_PHU PH
    WHERE PH.id IN (
        SELECT id
        FROM inserted
        UNION
        SELECT id
        FROM deleted
    );
END;

CREATE TRIGGER CheckSoLuongXuat
ON CT_PXH_PHU
AFTER INSERT, UPDATE
AS
BEGIN
    -- Kiểm tra và ngăn chặn việc thêm hoặc cập nhật CT_PXH_PHU với SoLuongXuat lớn hơn SoLuongTon
    IF EXISTS (
        SELECT *
        FROM CT_PXH_PHU CT
        INNER JOIN MATHANG MH ON CT.MaMH = MH.MaMH
        INNER JOIN inserted I ON CT.id = I.id AND CT.MaMH = I.MaMH
        WHERE I.SoLuongXuat > MH.SoLuongTon
    )
    BEGIN
        -- Hiển thị thông báo lỗi
        PRINT (N'Không thể thực hiện thao tác. Số lượng xuất vượt quá số lượng tồn')
		ROLLBACK
    END
END;

--=============================== BỘ NHỚ PHỤ NHẬP HÀNG ==========================================
CREATE TABLE PHIEUNHAPHANG_PHU
(
    id int IDENTITY(1,1) PRIMARY KEY,
	NgayNhap smalldatetime,
	TongTien money default 0
)

CREATE TABLE CT_PNH_PHU
(
	id int,
	MaMH char(5),
	SoLuongNhap int default 1 check (SoLuongNhap > 0),
	DonGiaNhap money default 0,
	ThanhTien money default 0,
    CONSTRAINT PK_PNH_phu PRIMARY KEY (id, MaMH)
)


CREATE TRIGGER UpdateTongTien_CT_PNH
ON CT_PNH_PHU
AFTER INSERT, UPDATE, DELETE
AS
BEGIN
    -- Cập nhật ThanhTien cho các CT_PNH_PHU
    UPDATE CT
    SET CT.ThanhTien = CT.SoLuongNhap * CT.DonGiaNhap
    FROM CT_PNH_PHU CT
    INNER JOIN inserted I ON CT.id = I.id AND CT.MaMH = I.MaMH
    WHERE I.id IS NOT NULL;

    -- Cập nhật TongTien trong PHIEUNHAPHANG_PHU
    UPDATE PH
    SET PH.TongTien = (
        SELECT SUM(CT.ThanhTien)
        FROM CT_PNH_PHU CT
        WHERE PH.id = CT.id
    )
    FROM PHIEUNHAPHANG_PHU PH
    WHERE PH.id IN (
        SELECT id
        FROM inserted
        UNION
        SELECT id
        FROM deleted
    );
END;


