create or replace function getfrequency(freq varchar(50))
 returns int language 'sql' 
 begin atomic 
 return (select id_frequency from frequency where name_frequency like '%'||freq||'%');
  end;

create  or replace function getparameters(param varchar(50))
 returns int language 'sql' 
 begin atomic 
 return (select id_parameters from parameters where name_parameters like '%'||param||'%');
  end;

create or replace procedure addvalue(freq varchar(50), param varchar(50), chanel int, dataval real)
 language 'sql' 
 begin atomic 
 insert into archive (id_frequency, id_parameters, chanel, datetime, mvkvalue) values (getfrequency(lower(addvalue.freq)), getparameters(addvalue.param), addvalue.chanel, now(), addvalue.dataval);
end;
