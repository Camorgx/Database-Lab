drop procedure if exists deleteLesson;

delimiter //

create procedure deleteLesson(
    in ID char(255),
    out result int
)
begin
    declare s int default 0;
    declare continue handler for sqlexception set s = 1;
    start transaction;
    set result = 0;
    delete from teach where lessonID = ID;
    delete from lesson where lessonID = ID;
    if s = 0 then commit;
    else
        set result = 1;
        rollback;
    end if;
end //

delimiter ;
