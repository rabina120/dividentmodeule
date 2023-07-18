(function ($) {
	$(window).on('load', function () {
		setTimeout(function () {
			$(".preloader").delay(15).fadeOut(500).addClass('loaded');
		}, 100);
	});
})(jQuery);


function Openloader() {
	$(".preloader").removeClass('loaded');
	
	$(".preloader").css('display', 'block')
	
}

function Closeloader() {
	$(".preloader").addClass('loaded');
	$(".preloader ").css('display', 'none')
}