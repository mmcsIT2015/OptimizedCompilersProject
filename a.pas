label A,B,C;

var 
	i,j: real;
	t, k, u, s, f: real;
begin
	i := 0;
	while (i < 100000000) do
	begin
		i := i + 1;
		j := j + 1;
		k := j + (t * s + 5.0*7.0);
		u := u * (t * s + 5.0*7.0) ;
		s := 8.0 / 9.0 * 10.0 / 5.0 * 7.0 / 3.0 * 4.0 / 7.0;
		t := s;
	end;

	k := t;
	write(k);
end.