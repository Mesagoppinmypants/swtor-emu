/*
* Copyright (C) 2008-2012 Emulator Nexus <http://emulatornexus.com//>
*
* This program is free software; you can redistribute it and/or modify it
* under the terms of the GNU General Public License as published by the
* Free Software Foundation; either version 3 of the License, or (at your
* option) any later version.
*
* This program is distributed in the hope that it will be useful, but WITHOUT
* ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or
* FITNESS FOR A PARTICULAR PURPOSE. See the GNU General Public License for
* more details.
*
* You should have received a copy of the GNU General Public License along
* with this program. If not, see <http://www.gnu.org/licenses/>.
*/

#include "../Swtor_Server.h"

class swzlib {
	z_stream mInboundStream;
	z_stream mOutboundStream;

public:
	swzlib();
	int swtor_decompress(Bytef *source, Bytef *dest, int sizeIn, int sizeOut);
	int swtor_compress(Byte* inbuf, int insize, Byte* outbuf, int outsize);
};