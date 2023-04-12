SELECT DISTINCT
    ID AS '读者号',
    name AS '读者姓名'
FROM Reader, Borrow, Reserve
WHERE 
    ID NOT IN (SELECT reader_ID FROM Borrow)
    AND ID NOT IN (SELECT reader_ID FROM Reserve);