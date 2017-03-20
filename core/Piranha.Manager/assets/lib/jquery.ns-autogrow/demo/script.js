$(function(){
  $('.example1 textarea').autogrow();
  $('.example2 textarea').autogrow({vertical: true, horizontal: false});
  $('.example3 textarea').autogrow({vertical: false, horizontal: true});
  $('.example4 textarea').autogrow({flickering: false});
});
