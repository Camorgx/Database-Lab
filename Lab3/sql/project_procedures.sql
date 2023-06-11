drop procedure if exists deleteProject;

delimiter //

create procedure deleteProject(
    in ID char(5),
    out result int
)
begin
    declare s int default 0;
    declare continue handler for sqlexception set s = 1;
    start transaction;
    set result = 0;
    delete from undertake where projectID = ID;
    delete from project where projectID = ID;
    if s = 0 then commit;
    else
        set result = 1;
        rollback;
    end if;
end //

delimiter ;
