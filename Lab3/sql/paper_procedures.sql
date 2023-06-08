drop procedure if exists deletePaper;

delimiter //

create procedure deletePaper(
    in ID char(5),
    out result int
)
begin
    declare s int default 0;
    declare continue handler for sqlexception set s = 1;
    start transaction;
    set result = 0;
    delete from publish where paperID = ID;
    delete from paper where paperID = ID;
    if s = 0 then commit;
    else
        set result = 1;
        rollback;
    end if;
end //

delimiter ;
