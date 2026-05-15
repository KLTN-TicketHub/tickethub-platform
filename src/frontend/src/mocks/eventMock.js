// ─── MOCK EVENT DATA ─────────────────────────────────────────────────────────
// This file contains pre-mapped event data in the unified schema format.
// When a real backend is ready, this file can be deleted entirely.
// The eventService.js will switch from importing this to calling the API.

function generateSlug(title) {
  return title
    .toLowerCase()
    .normalize("NFD")
    .replace(/[\u0300-\u036f]/g, "")
    .replace(/[^a-z0-9]+/g, "-")
    .replace(/(^-|-$)+/g, "");
}

function computeStatus(dateStart) {
  const now = new Date()
  const start = new Date(dateStart)
  if (now > start) return 'ended'
  return 'upcoming'
}

function mapLegacy(event, category) {
  let parsedDate = event.date
  try {
    const d = new Date(event.date)
    if (!isNaN(d.getTime())) parsedDate = d.toISOString().split('T')[0]
  } catch (e) { /* keep original */ }

  let minPrice = 0, maxPrice = 0
  if (event.tiers && event.tiers.length > 0) {
    const prices = event.tiers.map(t => t.price)
    minPrice = Math.min(...prices)
    maxPrice = Math.max(...prices)
  }

  const performers = []
  if (event.artist) performers.push({ name: event.artist, role: 'Artist' })
  if (event.instructor) performers.push({ name: event.instructor, role: 'Instructor' })
  if (event.teams) event.teams.forEach(t => performers.push({ name: t, role: 'Team' }))

  return {
    id: event.id,
    title: event.title,
    slug: generateSlug(event.title),
    description: event.description || '',
    category,
    performers,
    tiers: event.tiers || [],
    location: {
      name: event.location || event.venue || '',
      address: event.address || '',
      city: event.city || 'other'
    },
    dateStart: parsedDate,
    dateEnd: event.dateEnd || '',
    time: event.time || '',
    priceRange: { min: minPrice, max: maxPrice },
    image: event.image,
    featured: event.featured || false,
    status: computeStatus(parsedDate),
    hasSeatMap: event.hasSeatMap || false,
    seatMap: event.seatMap || null,
    createdAt: new Date().toISOString(),
    updatedAt: new Date().toISOString()
  }
}

// ─── RAW LEGACY DATA ─────────────────────────────────────────────────────────
const RAW = {
  concerts: [
    { id: "c1", title: "SƠN TÙNG M-TP LIVE CONCERT 2099", artist: "Sơn Tùng M-TP", date: "Mon, 15 Mar 2099", time: "19:30", location: "Mỹ Đình National Stadium, Hà Nội", city: "hanoi", price: "800,000đ", tiers: [{name: 'GA', price: 800000}, {name: 'VIP', price: 1500000}, {name: 'VVIP', price: 3000000}], genre: "Pop", image: "https://images.unsplash.com/photo-1540039155733-5bb30b53aa14?w=600&q=80", featured: true, hasSeatMap: true, seatMap: {
      rows: [
        { id: 'A', tier: 'VVIP', seats: [{ id: 'A1', status: 'available' }, { id: 'A2', status: 'sold' }, { id: 'A3', status: 'sold' }, { id: 'A4', status: 'available' }, { id: 'A5', status: 'available' }] },
        { id: 'B', tier: 'VIP', seats: [{ id: 'B1', status: 'available' }, { id: 'B2', status: 'reserved' }, { id: 'B3', status: 'available' }, { id: 'B4', status: 'available' }, { id: 'B5', status: 'available' }] },
        { id: 'C', tier: 'GA', seats: [{ id: 'C1', status: 'available' }, { id: 'C2', status: 'available' }, { id: 'C3', status: 'available' }, { id: 'C4', status: 'available' }, { id: 'C5', status: 'available' }, { id: 'C6', status: 'available' }] },
        { id: 'D', tier: 'GA', seats: [{ id: 'D1', status: 'available' }, { id: 'D2', status: 'sold' }, { id: 'D3', status: 'sold' }, { id: 'D4', status: 'available' }, { id: 'D5', status: 'available' }, { id: 'D6', status: 'available' }] }
      ]
    } },
    { id: "c2", title: "EDM STORM FESTIVAL", artist: "DJ Snake & Friends", date: "Sun, 22 Mar 2025", time: "20:00", location: "Landmark 81, Hồ Chí Minh", city: "hcm", price: "1,200,000đ", tiers: [{name: 'General', price: 1200000}, {name: 'VIP', price: 2500000}], genre: "EDM", image: "https://images.unsplash.com/photo-1574169208507-84376144848b?w=600&q=80", featured: true },
    { id: "c3", title: "INDIE NIGHTS VOL. 7", artist: "Various Artists", date: "Fri, 28 Mar 2025", time: "18:00", location: "Hanoi Rock City", city: "hanoi", price: "250,000đ", tiers: [{name: 'General', price: 250000}], genre: "Rock", image: "https://images.unsplash.com/photo-1516450360452-9312f5e86fc7?w=600&q=80", featured: false },
    { id: "c4", title: "MỸ TÂM ACOUSTIC NIGHT", artist: "Mỹ Tâm", date: "Sat, 5 Apr 2025", time: "20:00", location: "Grand Palace, Đà Nẵng", city: "danang", price: "600,000đ", tiers: [{name: 'Standard', price: 600000}, {name: 'VIP', price: 1200000}], genre: "Pop", image: "https://images.unsplash.com/photo-1501386761578-eac5c94b800a?w=600&q=80", featured: false },
    { id: "c5", title: "JAZZ UNDER THE STARS", artist: "Trần Mạnh Tuấn Quartet", date: "Sun, 13 Apr 2025", time: "19:00", location: "Opera House, Hồ Chí Minh", city: "hcm", price: "450,000đ", tiers: [{name: 'Premium', price: 450000}, {name: 'VIP', price: 800000}], genre: "Jazz", image: "https://images.unsplash.com/photo-1415201364774-f6f0bb35f28f?w=600&q=80", featured: false },
    { id: "c6", title: "ROCK IN SAIGON 2025", artist: "Microwave Band + Ngũ Cung", date: "Sat, 19 Apr 2025", time: "18:30", location: "Ký Con Rooftop, Hồ Chí Minh", city: "hcm", price: "300,000đ", tiers: [{name: 'General', price: 300000}], genre: "Rock", image: "https://images.unsplash.com/photo-1468164016595-6108e4c60c8b?w=600&q=80", featured: false },
    { id: "c7", title: "[BẾN THÀNH] Đêm nhạc Bùi Anh Tuấn – Lâm Bảo Ngọc", artist: "Bùi Anh Tuấn, Lâm Bảo Ngọc", date: "Wed, 25 Mar 2026", time: "20:00", location: "Nhà Hát Bến Thành, TP.HCM", city: "hcm", price: "500,000đ", tiers: [{name: 'Standard', price: 500000}], genre: "Pop", image: "https://images.unsplash.com/photo-1514525253161-7a46d19cd819?w=600&q=80", featured: false },
    { id: "c8", title: "[BẾN THÀNH] Đêm nhạc Hoàng Hải – Văn Mai Hương", artist: "Hoàng Hải, Văn Mai Hương", date: "Thu, 26 Mar 2026", time: "20:00", location: "Nhà Hát Bến Thành, TP.HCM", city: "hcm", price: "500,000đ", tiers: [{name: 'Standard', price: 500000}], genre: "Pop", image: "https://images.unsplash.com/photo-1470225620780-dba8ba36b745?w=600&q=80", featured: false },
    { id: "c9", title: "Xoay Tròn Concert – Hoàng Dũng", artist: "Hoàng Dũng", date: "Sat, 28 Mar 2026", time: "19:00", location: "TP.HCM", city: "hcm", price: "649,000đ", tiers: [{name: 'Standard', price: 649000}], genre: "Pop", image: "https://images.unsplash.com/photo-1493225457124-a1a2a5f5f9af?w=600&q=80", featured: false },
    { id: "c10", title: "Mr Siro – Fan Concert – Ai Cũng Giấu Trong Lòng Tảng Băng", artist: "Mr Siro", date: "Sat, 28 Mar 2026", time: "19:30", location: "Hà Nội", city: "hanoi", price: "800,000đ", tiers: [{name: 'Standard', price: 800000}], genre: "Pop", image: "https://images.unsplash.com/photo-1459749411175-04bf5292ceea?w=600&q=80", featured: true },
    { id: "c11", title: "[The Grand Ho Tram] Đêm Nhạc Đẳng Cấp Cùng Lệ Quyên", artist: "Lệ Quyên", date: "Sat, 28 Mar 2026", time: "20:00", location: "The Grand Ho Tram", city: "other", price: "1,130,000đ", tiers: [{name: 'Standard', price: 1130000}], genre: "Pop", image: "https://images.unsplash.com/photo-1510511459019-5efa32e85c7a?w=600&q=80", featured: false },
    { id: "c12", title: "Tan Tan Acoustic – DATE NIGHT", artist: "Tan Tan Acoustic", date: "Sat, 28 Mar 2026", time: "19:00", location: "Tan Tan Cafe", city: "other", price: "140,000đ", tiers: [{name: 'Standard', price: 140000}], genre: "Acoustic", image: "https://images.unsplash.com/photo-1511671782779-c97d3d27a1d4?w=600&q=80", featured: false },
    { id: "c13", title: "[Fantasy Show] Yêu Em Cả Trong Giấc Mơ", artist: "Đàm Vĩnh Hưng", date: "Sat, 28 Mar 2026", time: "20:00", location: "Fantasy", city: "other", price: "800,000đ", tiers: [{name: 'Standard', price: 800000}], genre: "Pop", image: "https://images.unsplash.com/photo-1533174000273-e31b62a4d957?w=600&q=80", featured: false },
    { id: "c14", title: "[BẾN THÀNH] Đêm nhạc Cẩm Ly Birthday – Special Guest: Quốc Đại", artist: "Cẩm Ly", date: "Sun, 29 Mar 2026", time: "20:00", location: "Nhà Hát Bến Thành, TP.HCM", city: "hcm", price: "500,000đ", tiers: [{name: 'Standard', price: 500000}], genre: "Pop", image: "https://images.unsplash.com/photo-1506157786151-b84915371059?w=600&q=80", featured: false },
    { id: "c15", title: "[CAT&MOUSE] MICROWAVE + SUBROTHERS BAND", artist: "Microwave", date: "Sun, 29 Mar 2026", time: "19:30", location: "Cat&Mouse", city: "other", price: "300,000đ", tiers: [{name: 'Standard', price: 300000}], genre: "Rock", image: "https://images.unsplash.com/photo-1498038432885-c6f3f1b912ee?w=600&q=80", featured: false },
    { id: "c16", title: "SƠN TÙNG M-TP – SKYLINE TOUR 2027", artist: "Sơn Tùng M-TP", date: "Sat, 15 Mar 2027", time: "19:30", location: "Sân vận động Mỹ Đình, Hà Nội", city: "hanoi", tiers: [{name: 'GA', price: 900000}, {name: 'VIP', price: 2000000}, {name: 'VVIP', price: 4500000}], genre: "Pop", image: "https://images.unsplash.com/photo-1540039155733-5bb30b53aa14?w=600&q=80", featured: true, description: "Sơn Tùng M-TP chính thức trở lại với Skyline Tour 2027 – concert lớn nhất trong sự nghiệp. Hơn 30 bài hát hit sẽ được trình diễn trên sân khấu hiện đại với công nghệ ánh sáng 3D mapping. Đây là cơ hội duy nhất để thưởng thức trực tiếp các ca khúc Hãy Trao Cho Anh, Chạy Ngay Đi, Muộn Rồi Mà Sao Còn trong không gian hoành tráng nhất." },
    { id: "c17", title: "EDM GALAXY FESTIVAL 2027", artist: "Martin Garrix, Zedd, KSHMR", date: "Sat, 12 Jul 2027", time: "18:00", location: "Phú Mỹ Hưng, Hồ Chí Minh", city: "hcm", tiers: [{name: 'General', price: 1500000}, {name: 'VIP', price: 3500000}, {name: 'Backstage', price: 8000000}], genre: "EDM", image: "https://images.unsplash.com/photo-1574169208507-84376144848b?w=600&q=80", featured: true, description: "Lễ hội EDM lớn nhất Đông Nam Á với sự góp mặt của Martin Garrix, Zedd và KSHMR. 3 sân khấu, 12 giờ âm nhạc không ngừng nghỉ. Trải nghiệm âm thanh Dolby Atmos ngoài trời đầu tiên tại Việt Nam với hệ thống loa 360 độ và màn hình LED khổng lồ." },
    { id: "c18", title: "ĐEN VÂU – LIVE IN HÀ NỘI 2027", artist: "Đen Vâu", date: "Fri, 20 Jun 2027", time: "19:00", location: "Cung Điền Kinh Quốc Gia, Hà Nội", city: "hanoi", tiers: [{name: 'Phổ thông', price: 500000}, {name: 'VIP', price: 1200000}], genre: "Rap", image: "https://images.unsplash.com/photo-1493225457124-a1a2a5f5f9af?w=600&q=80", featured: true, description: "Đen Vâu mang đến đêm nhạc đặc biệt với hơn 25 ca khúc từ các album Dongvui Unfest, và những bài hát chưa từng công bố. Khách mời bí ẩn sẽ xuất hiện trên sân khấu. Không gian thân mật, gần gũi – đúng chất Đen." },
  ],
  arts: [
    { id: "a1", title: "THE PHANTOM OF THE OPERA", type: "Musical", date: "Fri, 14 Mar 2025", location: "Nhà Hát Lớn Hà Nội", city: "hanoi", tiers: [{name: 'Standard', price: 500000}, {name: 'Premium', price: 1500000}], image: "https://images.unsplash.com/photo-1503095396549-807759245b35?w=600&q=80", featured: true, description: "Vở nhạc kịch huyền thoại The Phantom of the Opera lần đầu tiên được trình diễn tại Nhà Hát Lớn Hà Nội với dàn diễn viên quốc tế. Câu chuyện tình yêu bi thương dưới hầm mộ Paris sẽ được tái hiện với âm nhạc sống và dàn dựng hoành tráng." },
    { id: "a2", title: "TRUYỆN KIỀU – VỞ KỊCH", type: "Traditional Theatre", date: "Sat, 22 Mar 2025", location: "Nhà Hát Tuổi Trẻ", city: "hanoi", tiers: [{name: 'General', price: 200000}], image: "https://images.unsplash.com/photo-1558618666-fcd25c85cd64?w=600&q=80", featured: false },
    { id: "a3", title: "MODERN DANCE: FRAGMENTS", type: "Dance", date: "Sun, 30 Mar 2025", location: "IDECAF Theatre, Hồ Chí Minh", city: "hcm", tiers: [{name: 'General', price: 350000}], image: "https://images.unsplash.com/photo-1518834107812-67b0b7c58434?w=600&q=80", featured: false },
    { id: "a4", title: "HAMLET – SHAKESPEARE REIMAGINED", type: "Drama", date: "Fri, 11 Apr 2025", location: "L'Espace, Hà Nội", city: "hanoi", tiers: [{name: 'Standard', price: 400000}], image: "https://images.unsplash.com/photo-1485846234645-a62644f84728?w=600&q=80", featured: true },
    { id: "a5", title: "AO SHOW – LÀNG PHỐ", type: "Circus Arts", date: "Sat, 5 Apr 2025", location: "Saigon Opera House, Hồ Chí Minh", city: "hcm", tiers: [{name: 'Standard', price: 600000}, {name: 'Premium', price: 1500000}], image: "https://images.unsplash.com/photo-1551963831-b3b1ca40c98e?w=600&q=80", featured: false },
    { id: "a6", title: "WATER PUPPET SHOW", type: "Traditional", date: "Sat, 12 Apr 2025", location: "Thăng Long Water Puppet Theatre, Hà Nội", city: "hanoi", tiers: [{name: 'General', price: 150000}], image: "https://images.unsplash.com/photo-1507525428034-b723cf961d3e?w=600&q=80", featured: false },
    { id: "a7", title: "VIETCHARM: KHI DI SẢN HÓA VŨ ĐIỆU", type: "Cultural Show", date: "Wed, 25 Mar 2026", location: "Nhà Hát Hòa Bình, Hồ Chí Minh", city: "hcm", tiers: [{name: 'Standard', price: 1600000}], image: "https://images.unsplash.com/photo-1521336575822-6da63fb45455?w=600&q=80", featured: false },
    { id: "a8", title: "Chào Show", type: "Variety Show", date: "Wed, 25 Mar 2026", location: "Nhà Hát Thành Phố, Hồ Chí Minh", city: "hcm", tiers: [{name: 'Standard', price: 1040000}], image: "https://images.unsplash.com/photo-1583766395091-2eb9994ed094?w=600&q=80", featured: false },
    { id: "a9", title: "ART WORKSHOP 'UJI MATCHA CHEESECAKE TARTE'", type: "Workshop", date: "Thu, 26 Mar 2026", location: "Art Studio, Hồ Chí Minh", city: "hcm", tiers: [{name: 'Standard', price: 420000}], image: "https://images.unsplash.com/photo-1579954115545-a95591f28bfc?w=600&q=80", featured: false },
    { id: "a10", title: "GOm Show Hà Nội", type: "Variety Show", date: "Fri, 27 Mar 2026", location: "GOm Show, Hà Nội", city: "hanoi", tiers: [{name: 'Standard', price: 800000}], image: "https://images.unsplash.com/photo-1513364776144-60967b0f800f?w=600&q=80", featured: true },
    { id: "a11", title: "CIRQUE DU SOLEIL – LUZIA VIETNAM 2027", type: "Circus Arts", date: "Fri, 10 Oct 2027", location: "Nhà Thi Đấu Phú Thọ, Hồ Chí Minh", city: "hcm", tiers: [{name: 'Standard', price: 1200000}, {name: 'Premium', price: 2800000}, {name: 'VIP Front Row', price: 5000000}], image: "https://images.unsplash.com/photo-1551963831-b3b1ca40c98e?w=600&q=80", featured: true, description: "Cirque du Soleil mang chương trình LUZIA đến Việt Nam lần đầu tiên. Một hành trình siêu thực xuyên qua thế giới tưởng tượng của Mexico với hơn 40 nghệ sĩ xiếc đẳng cấp thế giới. Hiệu ứng nước, ánh sáng và âm nhạc sống tạo nên trải nghiệm không thể quên." },
  ],
  sports: [
    { id: "s1", title: "VLEAGUE: HCMC FC vs. HA NỘI FC", sport: "Football", date: "Sat, 15 Mar 2025", location: "Thống Nhất Stadium, Hồ Chí Minh", city: "hcm", tiers: [{name: 'Khán đài C/D', price: 120000}, {name: 'Khán đài A', price: 300000}], teams: ["HCMC FC", "Hà Nội FC"], image: "https://images.unsplash.com/photo-1508098682722-e99c43a406b2?w=600&q=80", featured: true, description: "Trận đại chiến V.League giữa HCMC FC và Hà Nội FC tại sân Thống Nhất. Hai đội bóng hàng đầu Việt Nam đối đầu trong trận cầu được mong chờ nhất mùa giải với hàng ngàn cổ động viên cuồng nhiệt." },
    { id: "s2", title: "VIETNAM OPEN TENNIS 2025", sport: "Tennis", date: "Mon, 17 Mar 2025", location: "Cần Thơ Tennis Club", city: "other", tiers: [{name: 'General', price: 80000}], teams: ["International Players"], image: "https://images.unsplash.com/photo-1554068865-24cecd4e34b8?w=600&q=80", featured: false },
    { id: "s3", title: "SAIGON HEAT vs. THANG LONG WARRIORS", sport: "Basketball", date: "Sun, 23 Mar 2025", location: "TDTT Phú Thọ, Hồ Chí Minh", city: "hcm", tiers: [{name: 'Standard', price: 100000}, {name: 'Courtside', price: 500000}], teams: ["Saigon Heat", "Thang Long Warriors"], image: "https://images.unsplash.com/photo-1546519638-68e109498ffc?w=600&q=80", featured: true },
    { id: "s4", title: "HANOI MARATHON 2025", sport: "Running", date: "Sun, 6 Apr 2025", location: "Hoàn Kiếm Lake, Hà Nội", city: "hanoi", tiers: [{name: '5KM', price: 350000}, {name: '21KM', price: 750000}, {name: '42KM', price: 1200000}], teams: ["Open Registration"], image: "https://images.unsplash.com/photo-1594882645126-14020914d58d?w=600&q=80", featured: false },
    { id: "s5", title: "VBA ALL-STAR GAME 2025", sport: "Basketball", date: "Sat, 12 Apr 2025", location: "Nhà Thi Đấu Hoàng Mai, Hà Nội", city: "hanoi", tiers: [{name: 'Standard', price: 200000}, {name: 'VIP', price: 800000}], teams: ["East All-Stars", "West All-Stars"], image: "https://images.unsplash.com/photo-1504450758481-7338eba7524a?w=600&q=80", featured: false },
    { id: "s6", title: "PPA ASIA 1000 – MB HANOI CUP 2026", sport: "Pickleball", date: "Wed, 1 Apr 2026", location: "Hà Nội", city: "hanoi", tiers: [{name: 'Standard', price: 130000}], teams: ["Các VĐV"], image: "https://images.unsplash.com/photo-1622227922682-bfb2b1eb3288?w=600&q=80", featured: false },
    { id: "s7", title: "Lion Championship 30 – 2026", sport: "MMA", date: "Sat, 18 Apr 2026", location: "Hồ Chí Minh", city: "hcm", tiers: [{name: 'Standard', price: 350000}], teams: ["Fighters"], image: "https://images.unsplash.com/photo-1555597673-b21d5c935865?w=600&q=80", featured: false },
    { id: "s8", title: "VIETNAM GRAND PRIX – F1 HÀ NỘI 2027", sport: "Formula 1", date: "Sun, 4 May 2027", time: "14:00", location: "Hà Nội Street Circuit", city: "hanoi", tiers: [{name: 'Grandstand', price: 2500000}, {name: 'VIP Lounge', price: 8000000}, {name: 'Paddock Club', price: 25000000}], teams: ["F1 Teams"], image: "https://images.unsplash.com/photo-1504450758481-7338eba7524a?w=600&q=80", featured: true, description: "Giải đua xe Công thức 1 chính thức trở lại Hà Nội! Trải nghiệm tốc độ, âm thanh và sự kịch tính của những chiếc xe F1 trên đường đua phố Hà Nội. Khu vực Fan Zone với hoạt động giải trí, giao lưu tay đua và trưng bày xe đua thật." },
    { id: "s9", title: "TECHCOMBANK HCMC MARATHON 2027", sport: "Running", date: "Sun, 12 Jan 2027", time: "04:30", location: "Dinh Độc Lập, Hồ Chí Minh", city: "hcm", tiers: [{name: '5KM', price: 400000}, {name: '10KM', price: 600000}, {name: '21KM', price: 900000}, {name: '42KM', price: 1500000}], teams: ["Open Registration"], image: "https://images.unsplash.com/photo-1594882645126-14020914d58d?w=600&q=80", featured: true, description: "Giải marathon quốc tế lớn nhất Việt Nam với hơn 15,000 vận động viên từ 50 quốc gia. Cung đường chạy qua các địa danh nổi tiếng TP.HCM: Dinh Độc Lập, Nhà thờ Đức Bà, Bến Nhà Rồng. Finisher medal thiết kế đặc biệt cho mùa giải 2027." },
  ],
  experiences: [
    { id: "e1", title: "MEKONG DELTA SUNSET CRUISE", date: "Sat, 5 Apr 2025", location: "Cần Thơ", city: "other", tiers: [{name: 'Standard', price: 850000}], image: "https://images.unsplash.com/photo-1552465011-b4e21bf6e79a?w=600&q=80", featured: true, description: "Du ngoạn hoàng hôn trên sông Mekong với thuyền truyền thống. Thưởng thức ẩm thực miền Tây và ngắm cảnh đồng bằng sông Cửu Long tuyệt đẹp." },
    { id: "e2", title: "HA LONG BAY OVERNIGHT KAYAK", date: "Sat, 12 Apr 2025", location: "Hạ Long, Quảng Ninh", city: "other", tiers: [{name: 'Package', price: 2500000}], image: "https://images.unsplash.com/photo-1573789267827-ee31edc02f97?w=600&q=80", featured: true, description: "Chèo kayak qua các hang động kỳ bí và nghỉ đêm trên vịnh Hạ Long. Trải nghiệm bơi lội, câu mực đêm và ngắm bình minh từ boong tàu." },
    { id: "e3", title: "SAIGON BY NIGHT VESPA TOUR", date: "Sat, 19 Apr 2025", location: "Hồ Chí Minh", city: "hcm", tiers: [{name: 'Standard', price: 450000}], image: "https://images.unsplash.com/photo-1551218372-a8789b81b253?w=600&q=80", featured: false },
    { id: "e4", title: "HỘI AN LANTERN MAKING CLASS", date: "Sat, 26 Apr 2025", location: "Hội An, Quảng Nam", city: "other", tiers: [{name: 'Class', price: 300000}], image: "https://images.unsplash.com/photo-1528360983277-13d401cdc186?w=600&q=80", featured: false },
    { id: "e5", title: "PHÚ QUỐC SNORKELING EXPEDITION", date: "Sat, 3 May 2025", location: "Phú Quốc, Kiên Giang", city: "other", tiers: [{name: 'Expedition', price: 650000}], image: "https://images.unsplash.com/photo-1544551763-46a013bb70d5?w=600&q=80", featured: false },
    { id: "e6", title: "SAIGON STREET FOOD & VESPA ADVENTURE 2027", date: "Sat, 15 Mar 2027", time: "17:00", location: "Quận 1, Hồ Chí Minh", city: "hcm", tiers: [{name: 'Solo Rider', price: 650000}, {name: 'Duo Package', price: 1100000}], image: "https://images.unsplash.com/photo-1551218372-a8789b81b253?w=600&q=80", featured: true, description: "Khám phá ẩm thực đường phố Sài Gòn trên những chiếc Vespa cổ điển. Tour 4 giờ dẫn bạn qua 6 quán ăn bí mật chỉ người địa phương mới biết. Thưởng thức bánh mì, phở, bún bò và hơn 10 món ăn đặc sản. Hướng dẫn viên song ngữ, bảo hiểm và nước uống đầy đủ." },
    { id: "e7", title: "ĐÀLẠT GLAMPING & STARGAZING 2027", date: "Fri, 11 Apr 2027", time: "15:00", location: "Langbiang, Đà Lạt", city: "other", tiers: [{name: 'Standard Tent', price: 1800000}, {name: 'Premium Bell Tent', price: 3200000}], image: "https://images.unsplash.com/photo-1504851149312-7a075b496cc7?w=600&q=80", featured: true, description: "Trải nghiệm cắm trại sang trọng (glamping) trên đỉnh Langbiang với lều tiện nghi 5 sao. Chương trình bao gồm BBQ nướng ngoài trời, kính thiên văn ngắm sao chuyên nghiệp, yoga buổi sáng và trekking rừng thông. Một đêm thoát khỏi thành phố ồn ào." },
    { id: "e8", title: "PHÚ QUỐC SCUBA DIVING FESTIVAL 2027", date: "Sat, 7 Jun 2027", time: "07:00", location: "An Thới, Phú Quốc", city: "other", tiers: [{name: 'Discover Scuba', price: 2200000}, {name: 'Certified Diver', price: 1500000}, {name: 'VIP Boat', price: 4500000}], image: "https://images.unsplash.com/photo-1544551763-46a013bb70d5?w=600&q=80", featured: true, description: "Lễ hội lặn biển quốc tế đầu tiên tại Phú Quốc với hơn 500 thợ lặn từ 20 quốc gia. Khám phá rạn san hô pristine, xác tàu đắm và cuộc sống biển phong phú tại quần đảo An Thới. Bao gồm thiết bị lặn, bữa trưa trên tàu và chứng nhận PADI." },
  ],
  workshops: [
    { id: "w1", title: "REACT ADVANCED PATTERNS 2025", instructor: "Nguyễn Văn Khoa", date: "Sat, 22 Mar 2025", location: "Toong Coworking, Hồ Chí Minh", city: "hcm", tiers: [{name: 'Ticket', price: 1200000}], image: "https://images.unsplash.com/photo-1517180102446-f3ece451e9d8?w=600&q=80", featured: true, description: "Workshop chuyên sâu về React: Server Components, Suspense patterns, và kiến trúc micro-frontend. Thực hành trực tiếp trên project thực tế." },
    { id: "w2", title: "WATERCOLOUR FOR BEGINNERS", instructor: "Lê Thị Hoa", date: "Sun, 23 Mar 2025", location: "Art Space, Hà Nội", city: "hanoi", tiers: [{name: 'Ticket', price: 350000}], image: "https://images.unsplash.com/photo-1541701494587-cb58502866ab?w=600&q=80", featured: false },
    { id: "w3", title: "VIETNAMESE COOKING MASTERCLASS", instructor: "Chef Minh Đức", date: "Sat, 29 Mar 2025", location: "Hoa Tuc Kitchen, Hồ Chí Minh", city: "hcm", tiers: [{name: 'Ticket', price: 750000}], image: "https://images.unsplash.com/photo-1466637574441-749b8f19452f?w=600&q=80", featured: true, description: "Học nấu 5 món Việt đặc trưng từ Chef Minh Đức: phở, bún bò Huế, bánh xèo, gỏi cuốn và chè. Nguyên liệu tươi từ chợ Bến Thành, mang về công thức bí truyền." },
    { id: "w4", title: "DIGITAL MARKETING BOOTCAMP", instructor: "Trần Huy Hoàng", date: "Sat, 5 Apr 2025", location: "Online / Zoom", city: "online", tiers: [{name: 'Ticket', price: 1800000}], image: "https://images.unsplash.com/photo-1432888498266-38ffec3eaf0a?w=600&q=80", featured: false },
    { id: "w5", title: "COFFEE TASTING & BREWING", instructor: "Ngọc Anh (Q Grader)", date: "Sun, 6 Apr 2025", location: "The Workshop Coffee, Hồ Chí Minh", city: "hcm", tiers: [{name: 'Ticket', price: 280000}], image: "https://images.unsplash.com/photo-1447933601403-0c6688de566e?w=600&q=80", featured: false },
    { id: "w6", title: "AI & MACHINE LEARNING BOOTCAMP 2027", instructor: "Dr. Phạm Quốc Cường", date: "Sat, 22 Mar 2027", time: "09:00", location: "Google for Startups Campus, Hồ Chí Minh", city: "hcm", tiers: [{name: 'Standard', price: 2500000}, {name: 'VIP (1-on-1)', price: 5000000}], image: "https://images.unsplash.com/photo-1555949963-aa79dcee981c?w=600&q=80", featured: true, description: "Bootcamp 2 ngày về AI thực chiến: từ Python cơ bản đến triển khai model Machine Learning. Xây dựng chatbot AI, hệ thống recommendation và computer vision. Mỗi học viên nhận chứng chỉ Google Cloud AI Associate và 3 tháng credits miễn phí." },
    { id: "w7", title: "POTTERY & CERAMICS – TÌM VỀ GỐM VIỆT 2027", instructor: "Nghệ nhân Lý Ngọc Minh", date: "Sun, 6 Apr 2027", time: "09:00", location: "Làng gốm Bát Tràng, Hà Nội", city: "hanoi", tiers: [{name: 'Half Day', price: 450000}, {name: 'Full Day + Firing', price: 850000}], image: "https://images.unsplash.com/photo-1565193566173-7a0ee3dbe261?w=600&q=80", featured: true, description: "Trải nghiệm nghề gốm truyền thống 600 năm tại làng Bát Tràng cùng Nghệ nhân Nhân dân Lý Ngọc Minh. Học kỹ thuật vuốt gốm trên bàn xoay, vẽ hoa văn men lam, và nung tác phẩm của chính bạn. Gói Full Day bao gồm bữa trưa truyền thống và tác phẩm nung hoàn chỉnh gửi về nhà." },
    { id: "w8", title: "STREET PHOTOGRAPHY MASTERCLASS 2027", instructor: "Nguyễn Thành Lộc (Nat Geo)", date: "Sat, 17 May 2027", time: "06:00", location: "Phố cổ Hà Nội", city: "hanoi", tiers: [{name: 'Standard', price: 1200000}, {name: 'Pro (Lightroom included)', price: 2000000}], image: "https://images.unsplash.com/photo-1452587925148-ce544e77e70d?w=600&q=80", featured: true, description: "Workshop nhiếp ảnh đường phố cùng nhiếp ảnh gia National Geographic Nguyễn Thành Lộc. Buổi sáng: thực hành chụp ảnh từ 6h tại 36 phố phường Hà Nội. Buổi chiều: hậu kỳ Lightroom và review portfolio cá nhân. Gói Pro bao gồm bản quyền Adobe Lightroom 1 năm." },
  ],
}

// ─── EXPORT PRE-MAPPED DATA ──────────────────────────────────────────────────
export const MOCK_EVENTS = [
  ...RAW.concerts.map(e => mapLegacy(e, 'concerts')),
  ...RAW.arts.map(e => mapLegacy(e, 'arts')),
  ...RAW.sports.map(e => mapLegacy(e, 'sports')),
  ...RAW.experiences.map(e => mapLegacy(e, 'experiences')),
  ...RAW.workshops.map(e => mapLegacy(e, 'workshops')),
]
