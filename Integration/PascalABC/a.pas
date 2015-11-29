label A,B,C;

var 
  i,j: integer;
  t,t1: real := 2+3;
begin
	i := 5;
  write(1+i,(t+2)*3);
  for var i := 2 to 5 do
  begin
    if (i mod 2 = 0) then 
    begin
      t := 5;
    end;
  end
end.